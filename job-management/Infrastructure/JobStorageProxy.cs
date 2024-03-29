using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.DependencyWaiting;
using Domain.JobStorage;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure
{

    public class JobStorageProxy : IJobStorage
    {
        private readonly HttpClient _httpClient;
        private readonly IStorageDependencyWaitingService _storageDependencyWaitingService;
        private readonly ILogger<JobStorageProxy> _logger;
        private readonly Uri _baseUri;
        private readonly Uri _addJobUri;

        private readonly Uri _updateJobUri;
        private readonly Uri _getJobUri;
        private JsonSerializerSettings _dateSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-ddTHH:mm:ss"
        };
        public JobStorageProxy(
            HttpClient httpClient,
            IStorageDependencyWaitingService storageDependencyWaitingService,
            IOptions<JobStorageOptions> jobStorageOptionsAccessor,
            ILogger<JobStorageProxy> logger)
        {
            _httpClient = httpClient;
            _storageDependencyWaitingService = storageDependencyWaitingService;
            _logger = logger;
            _baseUri = jobStorageOptionsAccessor.Value.BaseUri;
            _addJobUri = new Uri(_baseUri, jobStorageOptionsAccessor.Value.AddJobRoute);
            _updateJobUri = new Uri(_baseUri, jobStorageOptionsAccessor.Value.UpdateJobRoute);
            _getJobUri = new Uri(_baseUri, jobStorageOptionsAccessor.Value.GetJobRoute);

        }

        public async Task InsertNewJobAsync(Job job)
        {
            await WaitOnStorage();

            var jsonBody = JsonConvert.SerializeObject(job, _dateSettings);
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
            await WaitOnStorage();
            var route = _getJobUri.AbsolutePath
                .TrimEnd('/')
                + "/" + jobId.ToString();

            var url = new Uri(_baseUri, route);

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Adding data to storage failed: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            try

            {
                return JsonConvert.DeserializeObject<Job>(content, _dateSettings);
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Get Job config error. {error} \n Could not parse: {json}", jre.Message, content);
                throw new InvalidOperationException("Error while parsing job config");
            }
        }

        public async Task UpdateJobAsync(Job job)
        {
            await InsertNewJobAsync(job);
            //var jsonBody = JsonConvert.SerializeObject(job);
            //var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            //var response = await _httpClient.PutAsync(_updateJobUri, httpContent);

            //if (!response.IsSuccessStatusCode)
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    throw new InvalidOperationException($"Adding data to storage failed: {error}");
            //}
        }

        private async Task WaitOnStorage()
        {
            var shouldWait = true;
            while (shouldWait)
            {
                shouldWait = !await _storageDependencyWaitingService.IsDependencyReadyAsync();
                if (shouldWait)
                {
                    _logger.LogWarning("Storage not ready");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}