using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Infrastructure.ComponentManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class JobConfigStorageProxy : IJobConfigStorage, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobConfigStorageProxy> _logger;
        private readonly Uri _addJobConfigUri;
        private readonly Uri _updateJobConfigUri;
        private readonly Uri _getJobConfigUri;

        public JobConfigStorageProxy(HttpClient httpClient,
            IOptions<JobConfigStorageOptions> jobConfigStorageOptionsAccessor,
            ILogger<JobConfigStorageProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var baseUri = new Uri(jobConfigStorageOptionsAccessor.Value.BaseUri);
            _addJobConfigUri = new Uri(baseUri, jobConfigStorageOptionsAccessor.Value.AddJobConfigRoute);
            _updateJobConfigUri = new Uri(baseUri, jobConfigStorageOptionsAccessor.Value.UpdateJobConfigRoute);
            _getJobConfigUri = new Uri(baseUri, jobConfigStorageOptionsAccessor.Value.GetJobConfigRoute);
        }
        public async Task InsertNewJobConfigAsync(JobConfig jobConfig)
        {
            var jobConfigPayload = new JobConfigPayload
            {
                JobStatus = jobConfig.JobStatus.ToString().ToLower(),
                JobId = jobConfig.JobId,
                DataAcquirers = jobConfig.DataAcquirers.ToList(),
                DataAnalysers = jobConfig.DataAnalysers.ToList(),
                TopicQuery = jobConfig.TopicQuery
            };

            var jsonBody = JsonConvert.SerializeObject(jobConfigPayload);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_addJobConfigUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }
        }

        public async Task<JobConfig> GetJobConfigAsync(Guid jobId)
        {

            var url = _getJobConfigUri.AbsolutePath
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
                var payload = JsonConvert.DeserializeObject<JobConfigPayload>(content);
                if (Enum.TryParse(payload.JobStatus, out JobStatus parsed))
                {
                    return new JobConfig
                    {
                        JobStatus = parsed,
                        JobId = payload.JobId,
                        TopicQuery = payload.TopicQuery,
                        DataAnalysers = payload.DataAnalysers,
                        DataAcquirers = payload.DataAcquirers
                    };
                }
                _logger.LogError("Get job config error. Could not parse: {value}",
                    payload.JobStatus);
                throw new InvalidOperationException("Get job config error. Could not parse: " + payload.JobStatus);

            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task UpdateJobConfig(JobConfig jobConfig)
        {
            var jobConfigPayload = new JobConfigPayload
            {
                JobStatus = jobConfig.JobStatus.ToString().ToLower(),
                JobId = jobConfig.JobId,
                DataAcquirers = jobConfig.DataAcquirers.ToList(),
                DataAnalysers = jobConfig.DataAnalysers.ToList(),
                TopicQuery = jobConfig.TopicQuery
            };

            var jsonBody = JsonConvert.SerializeObject(jobConfigPayload);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_updateJobConfigUri, httpContent);

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
