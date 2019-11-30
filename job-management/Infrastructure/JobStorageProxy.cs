using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.JobStorage;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class JobStorageProxy : IJobStorage, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobStorageProxy> _logger;
        private readonly Uri _addJobUri;
        private readonly Uri _addJobConfigUri;
        private readonly Uri _updateJobUri;
        private readonly Uri _getJobUri;

        public JobStorageProxy(
            HttpClient httpClient, 
            IOptions<JobStorageOptions> jobStorageOptionsAccessor,
            ILogger<JobStorageProxy> logger)
        {
            _httpClient = httpClient;
            this._logger = logger;
            var baseUri = jobStorageOptionsAccessor.Value.BaseUri;
            _addJobUri = new Uri(baseUri, jobStorageOptionsAccessor.Value.AddJobRoute);
            _addJobConfigUri = new Uri(baseUri, jobStorageOptionsAccessor.Value.AddJobConfigRoute);
            _updateJobUri = new Uri(baseUri, jobStorageOptionsAccessor.Value.UpdateJobRoute);
            _getJobUri = new Uri(baseUri, jobStorageOptionsAccessor.Value.GetJobRoute);
        }

        public async Task InsertJobComponentConfig(JobComponentConfig jobConfig)
        {
            var jsonBody = JsonConvert.SerializeObject(jobConfig);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_addJobConfigUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding job to storage failed: {error}");
            }
        }

        public async Task InsertNewJobAsync(Job job)
        {
            var jsonBody = JsonConvert.SerializeObject(job);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_addJobUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding job to storage failed: {error}");
            }
        }

        public async Task<Job> GetJobAsync(Guid jobId)
        {
            var url = _getJobUri.AbsolutePath
                .TrimEnd('/');

            url += jobId.ToString();

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<Job>(content);
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task UpdateJobAsync(Job job)
        {
            var jsonBody = JsonConvert.SerializeObject(job);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_updateJobUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        
    }
}