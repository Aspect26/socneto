using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure
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

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public class ComponentStorageProxy : IComponentRegistry, IDisposable
    {
        public class SubscribedComponentPayloadObject
        {
            [JsonProperty("id")]
            public string ComponentId { get; set; }

            [JsonProperty("type")]
            public string ComponentType { get; set; }

            [JsonProperty("inputChannelName")]
            public string InputChannelName { get; set; }

            [JsonProperty("updateChannelName")]
            public string UpdateChannelName { get; set; }

            [JsonProperty("attributes")]
            public Dictionary<string, JObject> Attributes { get; set; }
        }


        private readonly HttpClient _httpClient;
        private readonly ILogger<ComponentStorageProxy> _logger;
        private readonly Uri _addComponentUri;
        private readonly Uri _getComponentUri;
        private ComponentIdentifiers _componentIdentifier;

        public ComponentStorageProxy(HttpClient httpClient,
            IOptions<ComponentStorageOptions> componentStorageOptionsAccessor,
            IOptions<ComponentIdentifiers> componentIdentifiers,
            ILogger<ComponentStorageProxy> logger)
        {
            _componentIdentifier = componentIdentifiers.Value;
            _httpClient = httpClient;
            _logger = logger;
            var baseUri = new Uri(componentStorageOptionsAccessor.Value.BaseUri);
            _addComponentUri = new Uri(baseUri, componentStorageOptionsAccessor.Value.AddOrUpdateComponentRoute);

            _getComponentUri = new Uri(baseUri, componentStorageOptionsAccessor.Value.GetComponentRoute);
        }

        public async Task<bool> AddOrUpdateAsync(ComponentRegistrationModel componentRegistrationModel)
        {
            var subscribedComponent = new SubscribedComponentPayloadObject
            {
                Attributes = componentRegistrationModel
                    .Attributes
                    .ToDictionary(r=>r.Key,r=>r.Value),
                ComponentId = componentRegistrationModel.ComponentId,
                ComponentType = componentRegistrationModel.ComponentType,
                InputChannelName = componentRegistrationModel.InputChannelId,
                UpdateChannelName = componentRegistrationModel.UpdateChannelId
            };
            
            var jsonBody = JsonConvert.SerializeObject(subscribedComponent);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync(_addComponentUri, httpContent);

            if (response.IsSuccessStatusCode)
            {
                // TODO validate response
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }
        
        public async Task<SubscribedComponent> GetComponentById(string componentId)
        {
            var getUriWithParams = _getComponentUri
                .AbsoluteUri
                .Replace("componentId",
                componentId);
            
            var response = await _httpClient.GetAsync(getUriWithParams);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    return ParseComponent(content);
                }
                catch (Exception e)
                {
                    var error = "Could not parse object '{json}' due to error  '{error}'";
                    _logger.LogError(error,
                        content,
                        e.Message);
                    throw new InvalidOperationException(string.Format(error,content,e.Message));
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }

        private static SubscribedComponent ParseComponent(string content)
        {
            var payload = JsonConvert.DeserializeObject<SubscribedComponentPayloadObject>(content);
            return new SubscribedComponent(
                payload.ComponentId,
                payload.ComponentType,
                payload.InputChannelName,
                payload.UpdateChannelName,
                payload.Attributes
            );
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
