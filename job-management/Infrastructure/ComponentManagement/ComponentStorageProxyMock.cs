using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Infrastructure.ComponentManagement
{
    public class ComponentStorageProxyMock : IComponentRegistry, IDisposable
    {
        private readonly HttpClient _client;
        private readonly ILogger<ComponentStorageProxyMock> _logger;

        public ComponentStorageProxyMock(
            HttpClient client,
            ILogger<ComponentStorageProxyMock> logger )
        {
            _client = client;
            _logger = logger;
        }
        public Task<bool> AddOrUpdateAsync(ComponentRegistrationModel componentRegistrationModel)
        {
            _logger.LogWarning($"Mock {nameof(AddOrUpdateAsync)} method was called");
            return Task.FromResult(true);
        }

        public Task<SubscribedComponent> GetComponentById(string componentId)
        {
            _logger.LogWarning($"Mock {nameof(GetComponentById)} method was called");
            var component =  new SubscribedComponent(
                "mock_cmp",
                "mock_type",
                "inputChannel",
                "updateChannel",
                new Dictionary<string, JObject>());
            return Task.FromResult(component);
        }

        public StorageComponent GetRegisteredStorage()
        {
            return new StorageComponent
            {
                AnalysedDataInputChannel = "analysedData",
                AcquiredDataInputChannel = "acquiredData"
            };
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}