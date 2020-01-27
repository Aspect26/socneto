using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Socneto.Domain.Models;
using Socneto.Infrastructure;
using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Domain.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _host;
        private readonly HttpClient _client = new HttpClient();
        private readonly ILogger<StorageService> _logger;
        
        private IList<SocnetoComponent> _cachedAnalysers = new List<SocnetoComponent>();  

        public StorageService(ILogger<StorageService> logger, IOptions<StorageOptions> storageOptionsObject)
        {
            _logger = logger;
            
            if (string.IsNullOrEmpty(storageOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(storageOptionsObject.Value.ServerAddress));
            
            _host = storageOptionsObject.Value.ServerAddress;
        }
        
        public async Task<User> GetUser(string username)
        {
            var response = await Get($"users?username={username}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<IList<Job>> GetUserJobs(string username)
        {
            var response = await Get($"jobs?username={username}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<Job>>();
        }

        public async Task<Job> GetJob(Guid jobId)
        {
            var response = await Get($"jobs/{jobId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<Job>();
        }

        public async Task<JobView> GetJobView(Guid jobId)
        {
            var response = await Get($"jobs/{jobId}/view");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<JobView>();
        }
        
        public async Task<JobView> StoreJobView(Guid jobId, JobView jobView)
        {
            var response = await Post($"jobs/{jobId}/view", jobView);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<JobView>();
        }

        public async Task<JobView> UpdateJobView(Guid jobId, JobView jobView)
        {
            var response = await Put($"jobs/{jobId}/view", jobView);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<JobView>();
        }

        public async Task<IList<AnalyzedPost>> GetAnalyzedPosts(Guid jobId, int offset, int size)
        {
            var response = await Get($"analyzedPosts?jobId={jobId}&offset={offset}&size={size}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsAsync<List<AnalyzedPost>>();
        }
        
        public async Task<IList<SocnetoComponent>> GetAnalysers()
        {
            var response = await Get($"components?type=DATA_ANALYSER");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            
            var analysers = JsonConvert.DeserializeObject<List<SocnetoComponent>>(responseString);
            _cachedAnalysers = analysers;
            return analysers;
        }
        
        public async Task<SocnetoComponent> GetAnalyser(string identifier)
        {
            var cachedAnalyser = _cachedAnalysers.FirstOrDefault(analyser => analyser.ComponentId == identifier);
            if (cachedAnalyser != null)
            {
                return cachedAnalyser;
            }

            _cachedAnalysers = await GetAnalysers();
            return _cachedAnalysers.FirstOrDefault(analyser => analyser.ComponentId == identifier);
        }

        public async Task<IList<SocnetoComponent>> GetAcquirers()
        {
            var response = await Get($"components?type=DATA_ACQUIRER");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<List<SocnetoComponent>>(responseString);
        }

        public async Task<AggregationAnalysisResult> GetAnalysisAggregation(GetAggregationAnalysisStorageRequest getAnalysisRequest)
        {
            var response = await Post($"results", getAnalysisRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<AggregationAnalysisResult>();
        }

        public async Task<ArrayAnalysisResult> GetAnalysisArray(GetArrayAnalysisStorageRequest getAnalysisRequest)
        {
            var response = await Post($"results", getAnalysisRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<ArrayAnalysisResult>();
        }

        private async Task<HttpResponseMessage> Get(string path)
        {
            var fullPath = GetFullPath(path);
            
            _logger.LogDebug($"GET {fullPath}");
            
            return await _client.GetAsync(fullPath);
        }
        
        private async Task<HttpResponseMessage> Post(string path, object data)
        {
            var fullPath = GetFullPath(path);
            var content = CreateHttpContent(data);
            
            _logger.LogDebug($"POST {fullPath} | {content}");
            
            return await _client.PostAsync(fullPath, content);
        }

        private async Task<HttpResponseMessage> Put(string path, object data)
        {
            var fullPath = GetFullPath(path);
            var content = CreateHttpContent(data);
            
            _logger.LogDebug($"PUT {fullPath} | {content}");
            
            return await _client.PutAsync(fullPath, content);
        }

        private string GetFullPath(string path)
        {
            return $"{_host}/{path}";
        }

        private HttpContent CreateHttpContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            _logger.LogInformation($"STORAGE REQUEST BODY: {json}");
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
