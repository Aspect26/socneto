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
        private readonly ILogger<StorageApiTester> _logger;

        public StorageApiTester(ILogger<StorageApiTester> logger)
        {
            _logger = logger;
        }

        public async Task TestAsync(IDataAcquirerMetadataStorage storage)
        {
            var twitterMetadata = new TwitterMetadata()
            {
                BatchSize = 1,
                Language = "en",
                MaxId = 111,
                Query = "q1",
                SinceId = 2
            };
            var jobId = Guid.NewGuid();
            await storage.SaveAsync(jobId, twitterMetadata);
            var retreived = await storage
                .GetAsync<TwitterMetadata>(jobId);

            var errors = AssertObject(twitterMetadata, retreived);
            AssertErrors("insert metadata", errors);
        }

        private IEnumerable<string> AssertObject(TwitterMetadata a, TwitterMetadata b)
        {
            yield return AssertProperty("BatchSize", r => r.BatchSize, a, b);

            yield return AssertProperty("Language", r => r.Language, a, b);

            yield return AssertProperty("MaxId", r => r.MaxId, a, b);

            yield return AssertProperty("Query", r => r.Query, a, b);

            yield return AssertProperty("SinceId", r => r.SinceId, a, b);
        }

        private string AssertProperty<TObj, TReturn>(
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
