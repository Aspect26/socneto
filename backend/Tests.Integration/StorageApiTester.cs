using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Socneto.Domain.Services;

namespace Tests.Integration
{
    public class StorageApiTester
    {
        private readonly IStorageService _storageService;
        private readonly ILogger<StorageApiTester> _logger;

        public StorageApiTester(
            IStorageService storageService,
            ILogger<StorageApiTester> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        public async Task TestAsync()
        {
            try
            {
                await TestIsComponentRunning();
                await TestGetUser();
            }
            catch (Exception e)
            {
                throw new BackendTestException("An exception occured during integration testing", e);
            }
        }

        private async Task TestIsComponentRunning()
        {
            var response = await _storageService.IsComponentRunning();
            AssertEqual(response, true, "The storage response should be 'is running'");
        }
        
        private async Task TestGetUser()
        {
            var response = await _storageService.GetUser("admin");
            AssertEqual(response, true, "The storage response should be 'is running'");
        }

        private static void AssertEqual(object v1, object v2, string message)
        {
            if (v1 != v2)
            {
                throw new BackendAssertException(message);
            }
        }

        public void AssertProperty<TObj, TReturn>(
            string propertyName,
            Func<TObj, TReturn> propertySelector,
            TObj a,
            TObj b)
        {
            var assertErrorMessage = "Different {0} a: {1} b: {2}";
            if (propertySelector(a)?.ToString() != propertySelector(b)?.ToString())
                throw new BackendAssertException(string.Format(
                    assertErrorMessage,
                    propertyName,
                    propertySelector(a),
                    propertySelector(b))
                );
        }

    }
}
