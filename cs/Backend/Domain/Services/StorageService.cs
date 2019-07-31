using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class StorageService : IStorageService
    {
        private const string Host = "http://localhost:8080";
        private readonly HttpClient _client = new HttpClient();
        
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

        private async Task<HttpResponseMessage> Get(string path)
        {
            return await _client.GetAsync($"{Host}/{path}");
        }
    }
}