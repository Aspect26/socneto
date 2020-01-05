using Domain;
using Domain.Acquisition;
using Domain.JobManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{

    //@GetMapping("/components/{componentId}/metadata")
    //@GetMapping("/components/{componentId}/metadata/job/{jobId}") ???
    //@PutMapping("/components/{componentId}/metadata")
    public class MetadataStorageProxyOptions
    {
        [Required]
        public Uri BaseUri { get; set; }

        [Required]
        public string GetComponentJobMetadataRoute { get; set; }
        
        [Required]
        public string PutComponentMetadataRoute { get; set; }

    }
    public class MetadataStorageProxy : IDataAcquirerMetadataStorage
    {
        private readonly string _componentId;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MetadataStorageProxy> _logger;
        private readonly MetadataStorageProxyOptions _routes;

        public MetadataStorageProxy(
            HttpClient httpClient,
            IOptions<ComponentOptions> coponentOptionsAccessor,
            IOptions<MetadataStorageProxyOptions> options,
            ILogger<MetadataStorageProxy> logger)
        {
            _componentId = coponentOptionsAccessor.Value.ComponentId;
            _httpClient = httpClient;
            _logger = logger;
            _routes = options.Value;
        }

        public async Task<T> GetAsync<T>(Guid jobId) where T : class,IDataAcquirerMetadata
        {
            var getRoute = string.Format(
                _routes.GetComponentJobMetadataRoute, 
                _componentId,
                jobId);

            var url = new Uri(_routes.BaseUri, getRoute);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task SaveAsync<T>(T metadata) where T : class,IDataAcquirerMetadata
        {
            var jsonBody = JsonConvert.SerializeObject(metadata);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_routes.PutComponentMetadataRoute, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }
    }

    public class TwitterJsonFileMetadataStorage : IDataAcquirerMetadataStorage
    {
        private readonly TwitterJsonStorageOptions _twitterJsonStorageOptions;
        private readonly DirectoryInfo _baseDirectory;
        private readonly object _rwLock = new object();
        public TwitterJsonFileMetadataStorage(
            IOptions<TwitterJsonStorageOptions> options)
        {
            _twitterJsonStorageOptions = options.Value;
            _baseDirectory = new DirectoryInfo(_twitterJsonStorageOptions.Directory);
        }

        public Task<T> GetAsync<T>(Guid jobId) where T : class,IDataAcquirerMetadata
        {

            var filePath = string.Format(_twitterJsonStorageOptions.FilePathTemplate, jobId);

            var fullName = Path.Combine(_baseDirectory.FullName, filePath);

            try
            {
                lock (_rwLock)
                {
                    using var reader = new StreamReader(fullName);
                    var metadata = reader.ReadToEnd();
                    try
                    {
                        var twitterMeta = JsonConvert.DeserializeObject<T>(metadata);
                        return Task.FromResult(twitterMeta);

                    }
                    catch (JsonReaderException)
                    {
                        throw new InvalidOperationException($"Invalid format of json {metadata}");
                    }
                }
            }
            catch (IOException)
            {
                return Task.FromResult<T>(null);
            }
        }

        public Task SaveAsync<T>(T metadata) where T :class, IDataAcquirerMetadata
        {
            var filePath = string.Format(
                _twitterJsonStorageOptions.FilePathTemplate, 
                metadata.JobId);
            var fullName = Path.Combine(_baseDirectory.FullName, filePath);
            lock (_rwLock)
            {
                using var writer = new StreamWriter(fullName);
                var metadataJson = JsonConvert.SerializeObject(metadata);
                writer.WriteLine(metadataJson);
            }

            return Task.CompletedTask;
        }
    }
}
