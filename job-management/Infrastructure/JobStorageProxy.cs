using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.Job;
using Domain.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class JobStorageProxy : IJobStorage, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _addJobUri;

        public JobStorageProxy(HttpClient httpClient, IOptions<JobStorageOptions> jobStorageOptionsAccessor)
        {
            _httpClient = httpClient;

            var baseUri = new Uri(jobStorageOptionsAccessor.Value.BaseUri);
            _addJobUri = new Uri(baseUri, jobStorageOptionsAccessor.Value.AddJobRoute);
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
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}