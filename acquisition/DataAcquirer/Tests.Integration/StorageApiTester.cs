using Domain.JobManagement;
using Infrastructure.Twitter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Integration
{
    public class StorageApiTester
    {
        private readonly IDataAcquirerMetadataStorage _dataAcquirerMetadataStorage;
        private readonly ILogger<StorageApiTester> _logger;

        public StorageApiTester(
            IDataAcquirerMetadataStorage dataAcquirerMetadataStorage,
            ILogger<StorageApiTester> logger)
        {
            _dataAcquirerMetadataStorage = dataAcquirerMetadataStorage;
            _logger = logger;
        }

        // TODO add attributes


        public async Task TestAsync()
        {
            IEnumerable<string> assert(TwitterMetadata a, TwitterMetadata b)
            {

                throw new NotImplementedException();
            }
            var twitterMetadata = new TwitterMetadata()
            {
                BatchSize = 1,
                //Credentials = new TwitterCredentials
                //{
                //    AccessToken = "a",
                //    AccessTokenSecret = "b",
                //    ConsumerKey = "c",
                //    ConsumerSecret = "d"
                //},
                Language = "en",
                MaxId = 111,
                Query = "q1",
                SinceId = 2
            };
            var jobId = Guid.NewGuid();
            await _dataAcquirerMetadataStorage.SaveAsync(jobId, twitterMetadata);
            var retreived = await _dataAcquirerMetadataStorage
                .GetAsync<TwitterMetadata>(jobId);

            var errors = assert(twitterMetadata, retreived);
            AssertErrors("insert metadata" ,errors);
        }



        public string AssertProperty<TObj, TReturn>(
            string propertyName,
            Func<TObj, TReturn> propertySelector,
            TObj a,
            TObj b)
        {
            var assertErrorMessage = "Different {0} a: {1} b: {2}";
            if (propertySelector(a)?.ToString() != propertySelector(b)?.ToString())
            {
                return string.Format(
                    assertErrorMessage,
                    propertyName,
                    propertySelector(a),
                    propertySelector(b));
            }
            return null;
        }
        private void AssertErrors(string name, IEnumerable<string> errors)
        {
            if (errors.Where(r => !(r is null)).Any())
            {
                var errorMessage = string.Join('\n', errors);
                _logger.LogError($"{name} failed. Errors: ", errorMessage);
                throw new AcquirerAssertException(errorMessage);
            }
        }


    }
}
