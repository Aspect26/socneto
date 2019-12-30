using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.ComponentManagement
{
    [SuppressMessage(
        "Design",
        "CA1063:Implement IDisposable Correctly",
        Justification = "In asp net app, http client is not supposed to be disposed")]
    public class ComponentStorageProxy : IComponentRegistry
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ComponentStorageProxy> _logger;
        private readonly StorageChannelNames _storageChannelNames;
        private readonly Uri _addComponentUri;
        private readonly Uri _getComponentUri;
        private readonly string _getComponentJobConfigUriTemplate;
        private readonly string _insertComponentJobConfigUriTemplate;
        private readonly ComponentIdentifiers _componentIdentifier;

        public ComponentStorageProxy(HttpClient httpClient,
            IOptions<ComponentStorageOptions> componentStorageOptionsAccessor,
            IOptions<StorageChannelNames> storageChannelNamesAccessor,
            IOptions<ComponentIdentifiers> componentIdentifiers,

            ILogger<ComponentStorageProxy> logger)
        {
            _componentIdentifier = componentIdentifiers.Value;
            _httpClient = httpClient;
            _logger = logger;
            _storageChannelNames = storageChannelNamesAccessor.Value;
            var baseUri = new Uri(componentStorageOptionsAccessor.Value.BaseUri);
            _addComponentUri = new Uri(baseUri, componentStorageOptionsAccessor.Value.AddOrUpdateComponentRoute);
            _getComponentUri = new Uri(baseUri, componentStorageOptionsAccessor.Value.GetComponentRoute);

            _getComponentJobConfigUriTemplate = baseUri.AbsoluteUri.TrimEnd('/') + componentStorageOptionsAccessor.Value.ComponentJobConfigRoute;
            _insertComponentJobConfigUriTemplate = baseUri.AbsoluteUri.TrimEnd('/') + componentStorageOptionsAccessor.Value.ComponentJobConfigRoute;
        }

        public async Task AddOrUpdateAsync(ComponentModel componentRegistrationModel)
        {
            var subscribedComponent = new SubscribedComponentPayloadObject
            {
                Attributes = componentRegistrationModel.Attributes,
                ComponentId = componentRegistrationModel.ComponentId,
                ComponentType = componentRegistrationModel.ComponentType,
                InputChannelName = componentRegistrationModel.InputChannelName,
                UpdateChannelName = componentRegistrationModel.UpdateChannelName
            };

            var jsonBody = JsonConvert.SerializeObject(subscribedComponent);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_addComponentUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed:code:{response.StatusCode} {error}");
            }
        }


        public async Task InsertJobComponentConfigAsync(JobComponentConfig jobConfig)
        {
            var jsonBody = JsonConvert.SerializeObject(jobConfig);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var jobConfigUri = new Uri(
                _insertComponentJobConfigUriTemplate
                .Replace("componentId", jobConfig.ComponentId));

            var response = await _httpClient.PostAsync(jobConfigUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding job to storage failed: {error}");
            }
        }

        public async Task<List<JobComponentConfig>> GetAllComponentJobConfigsAsync(string componentId)
        {
            var jobConfigUri = new Uri(
                _getComponentJobConfigUriTemplate
                .Replace("componentId", componentId));

            var response = await _httpClient.GetAsync(jobConfigUri);


            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding job to storage failed: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {

                return JsonConvert.DeserializeObject<List<JobComponentConfig>>(content);
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task<ComponentModel> GetComponentByIdAsync(string componentId)
        {
            var getUriWithParams = _getComponentUri
                .AbsoluteUri
                .Replace("componentId", componentId);

            var response = await _httpClient.GetAsync(getUriWithParams);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
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
                    throw new InvalidOperationException(string.Format(error, content, e.Message));
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }

        public StorageComponent GetRegisteredStorage()
        {
            return new StorageComponent
            {
                AnalysedDataInputChannel = _storageChannelNames.StoreAnalysedDataChannelName,
                AcquiredDataInputChannel = _storageChannelNames.StoreRawDataChannelName
            };
        }

        private static ComponentModel ParseComponent(string content)
        {
            var payload = JsonConvert.DeserializeObject<SubscribedComponentPayloadObject>(content);
            return new ComponentModel(
                payload.ComponentId,
                payload.ComponentType,
                payload.InputChannelName,
                payload.UpdateChannelName,
                payload.Attributes
            );
        }
    }
}
