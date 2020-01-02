using CsvHelper;
using System;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;
using System.IO;
using Domain.Model;
using Domain.Acquisition;
using System.Collections.Generic;
using Infrastructure.CustomStaticData.MappingAttributes;

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

            objectName = "tweets.apple.json";
            mappingName = "tweets.json.mapping";

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

    }
}
