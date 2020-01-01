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

namespace ConsoleApi.CustomStaticData
{
    public class CustomDataAttributes
    {
        [JsonProperty("hasHeaders")]
        public bool HasHeaders { get; set; }
        [JsonProperty("dataFormat")]
        public string DataFormat { get; set; }
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
    }

    public class CustomStaticDataAcquirer : IDataAcquirer
    {
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secret;
        private readonly string _bucketElementName;
        private readonly string _objectElementName;
        private readonly IEventTracker<CustomStaticDataAcquirer> _logger;

        public CustomStaticDataAcquirer(
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

            // TODO parse from file
            var atts = new CustomDataAttributes
            {
                HasHeaders = true,
                DataFormat = "csv"
            };

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

            var reader = GetReaderFromFormat(atts);

            using (var cts = new CancellationTokenSource())
            {
                var listeningTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        await minio.GetObjectAsync(bucketName, objectName, reader.StartPopulating);
                    }
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

        private static ICustomStreamReader GetReaderFromFormat(CustomDataAttributes atts)
        {
            ICustomStreamReader reader;
            if (atts.DataFormat == "csv")
            {
                reader = new CsvStreamReader(atts);
            }
            else if (atts.DataFormat == "json")
            {
                reader = new JsonStreamReader(atts);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported data format '{atts.DataFormat}'");
            }

            return reader;
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

        private readonly CustomDataAttributes _attributes;

        public CsvStreamReader(CustomDataAttributes attributes)
        {
            _attributes = attributes;
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

        public  void StartPopulating(Stream stream)
        {
            try
            {
                using (var se = new StreamReader(stream))
                using (var csvreader = new CsvReader(se))
                {
                    try
                    {
                        if (_attributes.HasHeaders)
                        {
                            csvreader.ReadHeader();
                        }
                    }
                    catch (ReaderException)
                    {
                        // TODO log that no header was found
                    }

                    while (csvreader.Read())
                    {
                        if (csvreader.TryGetField<string>(5, out var text))
                        {
                            //Console.WriteLine($"{count}:{text}");
                        }
                        else
                        {
                            Console.Error.WriteLine("Could not read");
                        }
                        var originalPostId = "n/a";

                        var language = "en";
                        var source = "custom_data";
                        var userId = "n/a";
                        var dateTimeString = DateTime.Now.ToString("s");
                        var dataAcquirerPost = DataAcquirerPost.FromValues(
                            originalPostId,
                            text,
                            language,
                            source,
                            userId,
                            dateTimeString);

                        _posts.Enqueue(dataAcquirerPost);
                    }

                }
                //var originalPostId = "n/a";

                //while (true)
                //{
                //    var language = "en";
                //    var source = "custom_data";
                //    var userId = "n/a";
                //    var dateTimeString = DateTime.Now.ToString("s");
                //    var dataAcquirerPost = DataAcquirerPost.FromValues(
                //        originalPostId,
                //        "test",
                //        language,
                //        source,
                //        userId,
                //        dateTimeString);
                //    var limit = 1000;
                //    while (limit < _posts.Count)
                //    {
                //        await Task.Delay(TimeSpan.FromSeconds(.5));
                //    }

                //    _posts.Enqueue(dataAcquirerPost);
                //}
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
    }
    public class JsonStreamReader : ICustomStreamReader
    {
        private readonly CustomDataAttributes _customDataAttributes;

        public JsonStreamReader(CustomDataAttributes customDataAttributes)
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
