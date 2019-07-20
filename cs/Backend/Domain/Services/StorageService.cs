using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Socneto.Domain.Models;


namespace Socneto.Domain.Services
{
    public class StorageService : IStorageService
    {
        private const string Host = "http://localhost:5001";
        private readonly HttpClient _client = new HttpClient();
        
        public async Task<User> GetUser(int userId)
        {
            var response = await Get($"users?userId={userId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<List<JobStatus>> GetUserJobs(int userId)
        {
            var response = await Get($"jobs?userId={userId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<JobStatus>>();
        }

        public async Task<JobStatus> GetJob(string jobId)
        {
            var response = await Get($"jobs/{jobId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<JobStatus>();
        }

        public async Task<List<Post>> GetAnalyzedPosts(string jobId)
        {
            var response = await Get($"analyzedPosts?jobId={jobId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<Post>>();
        }

        private async Task<HttpResponseMessage> Get(string path)
        {
            return await _client.GetAsync($"{Host}/{path}");
        }
    }
}