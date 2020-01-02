using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Domain;
using Domain.Acquisition;
using Domain.Model;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApi.CustomStaticData
{
    public class CsvMappingAttributes
    {
        [JsonProperty("hasHeaders")]
        public bool HasHeaders { get; set; }

        [JsonProperty("fixedValues")]
        public Dictionary<string, string> FixedValues { get; set; }

        [JsonProperty("indices")]
        public Dictionary<string, int> Indices { get; set; }
    }

    public class MappingAttributesRoot
    {

        [JsonProperty("dataFormat")]
        public string DataFormat { get; set; }

        [JsonProperty("mappingAttributes")]
        public JObject MappingAttributes { get; set; }
    }

    public class MinioOptions
    {
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string AccessKey { get; set; }
        [Required]
        public string SecretKey { get; set; }
    }
    public class AttributeElementNames
    {
        [Required]
        public string BucketElementName { get; set; }
        [Required]
        public string ObjectElementName { get; set; }
        [Required]
        public string MappingElementName { get; set; }
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


        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(DataAcquirerInputModel acquirerInputModel)
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
            catch (MinioException e)
            {
                _logger.TrackError(
                    "CustomDataAcquirer",
                    $"Object '{bucketName}-{objectName}' does not exist",
                    new
                    {
                        bucketName,
                        objectName,
                        exception = e
                    });
                yield break;
            }

            var reader = _customStreamReaderFactory.Create(atts);

            using (var cts = new CancellationTokenSource())
            {
                var listeningTask = Task.Run(async () =>
                {
                    await minio.GetObjectAsync(bucketName, objectName, reader.StartPopulating)
                    .ConfigureAwait(false);

                });

                while (!reader.ReadingEnded)
                {
                    if (reader.TryGetPost(out var post))
                    {
                        yield return post;
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
    public class CustomStreamReaderFactory
    {
        private readonly IEventTracker<CsvStreamReader> _csvEventTracker;

        public CustomStreamReaderFactory(
            IEventTracker<JsonStreamReader> jsonEventTracker,
            IEventTracker<CsvStreamReader> csvEventTracker)
        {
            _csvEventTracker = csvEventTracker;
        }
        public ICustomStreamReader Create(MappingAttributesRoot attributes)
        {
            try
            {
                if (attributes.DataFormat == "csv")
                {
                    var csvAttributes = attributes.MappingAttributes.ToObject<CsvMappingAttributes>();
                    return new CsvStreamReader(csvAttributes, _csvEventTracker);
                }
                else if (attributes.DataFormat == "json")
                {
                    return new JsonStreamReader(attributes);
                }
                else
                {
                    var message = $"Unsupported data format '{attributes.DataFormat}'";
                    throw new InvalidOperationException(message);
                }
            }
            catch (JsonException)
            {
                // TODO track error
                throw;
            }

        }

    }

    public interface ICustomStreamReader
    {
        void StartPopulating(Stream stream);
        bool ReadingEnded { get; }

        bool TryGetPost(out DataAcquirerPost post);
    }

    public class CsvStreamReader : ICustomStreamReader
    {
        private readonly ConcurrentQueue<DataAcquirerPost>
            _posts = new ConcurrentQueue<DataAcquirerPost>();

        private readonly CsvMappingAttributes _attributes;
        private readonly IEventTracker<CsvStreamReader> _eventTracker;

        public CsvStreamReader(CsvMappingAttributes attributes,
            IEventTracker<CsvStreamReader> eventTracker)
        {
            _attributes = attributes;
            _eventTracker = eventTracker;
        }

        public bool ReadingEnded { get; private set; } = false;

        public bool TryGetPost(out DataAcquirerPost post)
        {
            if (ReadingEnded)
            {
                post = null;
                return false;
            }
            return _posts.TryDequeue(out post);
        }

        public void StartPopulating(Stream stream)
        {
            try
            {
                using var se = new StreamReader(stream);
                using var csvreader = new CsvReader(se);

                if (_attributes.HasHeaders)
                {
                    csvreader.Read();
                }

                while (csvreader.Read())
                {
                    var dataAcquirerPost = ParsePostFromReader(csvreader);

                    while (_posts.Count > 1000)
                    {
                        // TODO God forgive me for I have sinned
                        // If anyone know how to implement backpressure,
                        // please let me know
                        Task.Delay(10).GetAwaiter().GetResult();
                    }
                    _posts.Enqueue(dataAcquirerPost);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReadingEnded = true;
            }

        }

        private DataAcquirerPost ParsePostFromReader(CsvReader csvreader)
        {
            var mutablePost = new JObject();
            mutablePost.TryAdd("source", "CustomStaticData");
            foreach (var (k, v) in _attributes.FixedValues)
            {
                mutablePost.TryAdd(k, v);
            }
            foreach (var (k, index) in _attributes.Indices)
            {
                if (csvreader.TryGetField<string>(index, out var value))
                {
                    mutablePost.TryAdd(k, value);
                }
            }
            var post = mutablePost.ToObject<MutableDataAcquirerPost>();
            var dataAcquirerPost = post.Freeze();
            return dataAcquirerPost;
        }
    }
    public class MutableDataAcquirerPost
    {
        [JsonProperty("originalPostId")]
        public string OriginalPostId { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("authorId")]
        public string UserId { get; set; }

        [JsonProperty("dateTime")]
        public string PostDateTime { get; set; }

        public DataAcquirerPost Freeze()
        {
            var datetime = PostDateTime ?? DateTime.Now.ToString("s");
            return DataAcquirerPost.FromValues(
                            OriginalPostId,
                            Text,
                            Language,
                            Source,
                            UserId,
                            datetime,
                            Query);

        }
    }

    public class JsonStreamReader : ICustomStreamReader
    {
        private readonly MappingAttributesRoot _customDataAttributes;

        public JsonStreamReader(MappingAttributesRoot customDataAttributes)
        {
            _customDataAttributes = customDataAttributes;
        }
        public bool ReadingEnded => throw new NotImplementedException();

        public void StartPopulating(Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPost(out DataAcquirerPost post)
        {
            throw new NotImplementedException();
        }
    }
}
