using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.Model;
using Infrastructure.CustomStaticData.MappingAttributes;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{

    public class PostValidator
    {
        private const string _validationErrorsFound = "The following errors were found parsing a post:\n{0}";
        private const string _errorEmpty = "Field '{0}' must not be null";

        public static OkErrorResult ValidatePost(DataAcquirerPost post)
        {
            IEnumerable<string> enumerateErrors()
            {
                if (string.IsNullOrEmpty(post.OriginalPostId))
                {
                    yield return string.Format(_errorEmpty, post.OriginalPostId);
                }
                if (string.IsNullOrEmpty(post.DateTime))
                {
                    yield return string.Format(_errorEmpty, post.DateTime);
                }
            }
            var validationsErrors = enumerateErrors();
            if (validationsErrors.Any())
            {
                var errors = string.Join("\n", validationsErrors);
                var errorMessage = string.Format(_validationErrorsFound, errors);
                return OkErrorResult.Error(errorMessage);
            }
            return OkErrorResult.Successful();
        }

    }
    public class CustomStaticDataAcquirer : IDataAcquirer
    {
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secret;
        private readonly string _bucketElementName;
        private readonly string _objectElementName;
        private readonly string _mappingElementName;
        private readonly CustomStreamReaderFactory _customStreamReaderFactory;
        private readonly IEventTracker<CustomStaticDataAcquirer> _logger;

        public CustomStaticDataAcquirer(
            CustomStreamReaderFactory customStreamReaderFactory,
            IOptions<AttributeElementNames> attributeElementNamesAcessor,
            IOptions<MinioOptions> customAcquirerOptionsAccessor,
            IEventTracker<CustomStaticDataAcquirer> logger)
        {

            var minioOptions = customAcquirerOptionsAccessor.Value;
            _endpoint = minioOptions.Endpoint;
            _accessKey = minioOptions.AccessKey;
            _secret = minioOptions.SecretKey;

            var attributeNames = attributeElementNamesAcessor.Value;
            _bucketElementName = attributeNames.BucketElementName;
            _objectElementName = attributeNames.ObjectElementName;
            _mappingElementName = attributeNames.MappingElementName;
            _customStreamReaderFactory = customStreamReaderFactory;
            _logger = logger;
        }


        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel,
            [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var minio = new MinioClient(
                _endpoint,
                _accessKey,
                _secret);

            var attributes = acquirerInputModel.Attributes;
            var bucketName = attributes.GetValue(_bucketElementName, null);
            var objectName = attributes.GetValue(_objectElementName, null);
            var mappingName = attributes.GetValue(_mappingElementName, null);

            var atts = await GetMappingAttributesAsync(minio, bucketName, mappingName);
            if (atts == null
                || string.IsNullOrEmpty(atts.DataFormat)
                || atts.MappingAttributes == null)
            {
                throw new InvalidOperationException("Invalid config");
            }

            try
            {
                await minio.StatObjectAsync(bucketName, objectName);
            }
            catch (MinioException)
            {
                //_logger.TrackError(
                //    "CustomDataAcquirer",
                //    $"Object '{bucketName}-{objectName}' does not exist",
                //    new
                //    {
                //        bucketName,
                //        objectName,
                //        exception = e
                //    });
                throw new InvalidOperationException($"Mapping object {mappingName} does not exits");
            }

            var reader = _customStreamReaderFactory.Create(atts);

            using (var cts = new CancellationTokenSource())
            {
                var listeningTask = Task.Run(async () =>
                {
                    await minio.GetObjectAsync(bucketName, objectName, reader.StartPopulating)
                    .ConfigureAwait(false);

                });

                while (!reader.IsCompleted)
                {
                    if (reader.TryGetPost(out var post))
                    {
                        var validation = PostValidator.ValidatePost(post);
                        if (validation.IsSuccessful)
                        {
                            yield return post;
                        }
                        else
                        {
                            _logger.TrackWarning(
                                "CustomStaticData",
                                "Invalid post encountered",
                                new
                                {
                                    errorMessage = validation.ErrorMessage,
                                    post
                                });
                        }
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                cts.Cancel();
                try
                {
                    await listeningTask;
                }
                catch (TaskCanceledException) { }
            }
        }

        private async Task<MappingAttributesRoot> GetMappingAttributesAsync(
            MinioClient minio,
            string bucketName,
            string mappingName)
        {
            try
            {
                await minio.StatObjectAsync(bucketName, mappingName);
            }
            catch (MinioException e)
            {
                _logger.TrackError(
                    "CustomDataAcquirer",
                    $"Missing mapping object '{bucketName}-{mappingName}'",
                    new
                    {
                        bucketName,
                        mappingName,
                        exception = e
                    });
                return null;
            }

            try
            {
                return await ParseMappingAttributesAsync(minio, bucketName, mappingName);
            }
            catch (JsonException e)
            {
                _logger.TrackError(
                   "CustomDataAcquirer",
                   $"could not parse json from '{bucketName}-{mappingName}'",
                   new
                   {
                       bucketName,
                       mappingName,
                       exception = e
                   });
                return null;
            }
        }

        private static async Task<MappingAttributesRoot> ParseMappingAttributesAsync(
            MinioClient minio,
            string bucketName,
            string mappingName)
        {
            using (var ms = new MemoryStream())
            {
                await minio.GetObjectAsync(bucketName, mappingName,
                    (s) =>
                    {
                        s.CopyTo(ms);
                    }
                );
                ms.Seek(0, SeekOrigin.Begin);
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(ms))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<MappingAttributesRoot>(jsonTextReader);
                }
            }
        }

    }
}
