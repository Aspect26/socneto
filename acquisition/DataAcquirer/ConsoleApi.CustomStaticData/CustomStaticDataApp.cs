using CsvHelper;
using System;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;
using System.IO;
using Domain.Model;
using Domain.Acquisition;
using System.Collections.Generic;

namespace ConsoleApi.CustomStaticData
{
    public class CustomStaticDataApp
    {
        private readonly IDataAcquirer _dataAcquirer;

        public CustomStaticDataApp(
            IDataAcquirer dataAcquirer)
        {
            _dataAcquirer = dataAcquirer;
        }
        public async Task DoAsync()
        {
            //await DoStaticTest();

            var bucketName = "data1";
            var objectName = "tweets_tiny_no_headers.csv";
            objectName = "tweets_tiny.csv";
            objectName = "tweets.csv";
            var mappingName = "tweets.csv.mapping";

            var attributesDict = new Dictionary<string, string>{
                    { "bucketName",bucketName },
                    {  "objectName",objectName },
                    {"mappingName",mappingName }
                };
            var attributes = new DataAcquirerAttributes(attributesDict);
            var guid = Guid.NewGuid();
            var daInput = new DataAcquirerInputModel(
                guid,
                null,
                null,
                attributes,
                0,
                0,
                0
                );
            var posts = _dataAcquirer.GetPostsAsync(daInput);
            await foreach (var item in posts)
            {
                Console.WriteLine(item.Text);
            }

        }

        private static async Task DoStaticTest()
        {
            //var endpoint = "http://172.17.0.2:9000";
            var endpoint = "127.0.0.1:9000";
            var accessKey = "minioadmin";
            var secretKey = "minioadmin";


            var minio = new MinioClient(
                endpoint,
                accessKey,
                secretKey);

            var bucketName = "data1";
            var objectName = "tweets_tiny_no_headers.csv";
            objectName = "tweets_tiny.csv";
            objectName = "tweets.csv";

            var atts = new MappingAttributesRoot
            {
                //    HasHeaders = true
            };
            try
            {
                var metadata = await minio.StatObjectAsync(bucketName, objectName);

                // Get input stream to have content of 'my-objectname' from 'my-bucketname'
                await minio.GetObjectAsync(bucketName, objectName,
                    (stream) =>
                    {
                        using (var se = new StreamReader(stream))
                        using (var csvreader = new CsvReader(se))
                        {
                            try
                            {
                                //if (atts.HasHeaders)
                                //{
                                //    csvreader.ReadHeader();
                                //}
                            }
                            catch (ReaderException)
                            {
                                // TODO log that no header was found
                            }
                            long count = 0;
                            while (csvreader.Read())
                            {
                                if (csvreader.TryGetField<string>(5, out var text))
                                {
                                    Console.WriteLine($"{count}:{text}");
                                    count++;
                                }
                                else
                                {
                                    Console.Error.WriteLine("Could not read");
                                }
                            }
                        }
                    });
            }
            catch (MinioException e)
            {
                // TODO logging
                Console.WriteLine("Error occurred: " + e);
            }
            catch (Exception)
            {
                // TODO log
                throw;
            }
        }
    }
}
