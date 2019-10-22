using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Domain.Models;
using Socneto.Infrastructure;

namespace Socneto.Domain.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _host;
        private readonly HttpClient _client = new HttpClient();
        private readonly ILogger<StorageService> _logger;

        public StorageService(ILogger<StorageService> logger, IOptions<StorageOptions> storageOptions)
        {
            _logger = logger;
            _host = storageOptions.Value.ServerAddress;
        }
        
        public async Task<User> GetUser(string username)
        {
            var response = await Get($"users?userId={username}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<IList<JobStatus>> GetUserJobs(string username)
        {
            var response = await Get($"jobs?userId={username}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<JobStatus>>();
        }

        public async Task<JobStatus> GetJob(Guid jobId)
        {
            var response = await Get($"jobs/{jobId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<JobStatus>();
        }

        public async Task<IList<AnalyzedPost>> GetAnalyzedPosts(Guid jobId)
        {
            var response = await Get($"analyzedPosts?jobId={jobId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<AnalyzedPost>>();
        }

        public async Task<IList<SocnetoComponent>> GetAnalysers()
        {
            var response = await Get($"components?type=DATA_ANALYSER");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<SocnetoComponent>>();
        }

        public async Task<IList<SocnetoComponent>> GetAcquirers()
        {
            var response = await Get($"components?type=DATA_ACQUIRER");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<SocnetoComponent>>();
        }

        private async Task<HttpResponseMessage> Get(string path)
        {
            return await _client.GetAsync($"{_host}/{path}");
        }
    }
}
