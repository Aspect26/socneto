using Domain.DependencyWaiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.DependencyWaiting
{

    public class StorageDependencyWaitingService : IStorageDependencyWaitingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StorageDependencyWaitingService> _logger;
        private Uri _healthcheckUri;

        public StorageDependencyWaitingService(
            HttpClient httpClient,
            IOptions<StorageServiceHealtcheckOptions> options,
            ILogger<StorageDependencyWaitingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var baseUri = options.Value.BaseUri;
            var route = options.Value.HealthCheckEndpoint;
            _healthcheckUri = new Uri(baseUri, route);
        }

        public async Task<bool> IsDependencyReadyAsync()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(_healthcheckUri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "Storage not ready: {responseCode}, {response}",
                        response.StatusCode,
                        content);
                    return false;
                }
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

    }
}
