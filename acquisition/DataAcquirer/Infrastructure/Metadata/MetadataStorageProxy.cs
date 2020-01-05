using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.JobManagement;
using Infrastructure.Twitter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Metadata
{
    public class MetadataStorageProxy : IDataAcquirerMetadataStorage
    {
        class MetadataWrapper
        {
            [JsonProperty(PropertyName = "componentId")]
            public string ComponentId { get; set; }

            [JsonProperty(PropertyName = "jobId")]
            public Guid JobId { get; set; }

            [JsonProperty(PropertyName = "componentMetadata")]
            public JObject ComponentMetadata { get; set; }
        }

        private readonly string _componentId;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MetadataStorageProxy> _logger;
        private readonly MetadataStorageProxyOptions _routes;

        public MetadataStorageProxy(
            IHttpClientFactory httpClientFactory,
            IOptions<ComponentOptions> coponentOptionsAccessor,
            IOptions<MetadataStorageProxyOptions> options,
            ILogger<MetadataStorageProxy> logger)
        {
            _componentId = coponentOptionsAccessor.Value.ComponentId;
            _httpClient = httpClientFactory.CreateClient(nameof(MetadataStorageProxy));
            _logger = logger;
            _routes = options.Value;
        }

        public async Task<T> GetAsync<T>(Guid jobId)
            where T : class, IDataAcquirerMetadata
        {
            var uri = new Uri(
                _routes.BaseUri,
                _routes.GetComponentJobMetadataRoute
                    .Replace("componentId", _componentId)
                    .Replace("jobId", jobId.ToString()));


            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var metadata = JsonConvert.DeserializeObject<MetadataWrapper>(content);
                return metadata.ComponentMetadata.ToObject<T>();
            }
            catch (JsonException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task SaveAsync<T>(Guid jobId, T metadata)
            where T : class, IDataAcquirerMetadata
        {
            var metadataWrapper = new MetadataWrapper
            {
                ComponentId = _componentId,
                JobId = jobId,
                ComponentMetadata = JObject.FromObject(metadata)
            };

            var jsonBody = JsonConvert.SerializeObject(metadataWrapper);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var uri = new Uri(
                _routes.BaseUri,
                _routes.PostComponentMetadataRoute.Replace("componentId", _componentId));

            var response = await _httpClient
                .PostAsync(uri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }
    }
}
