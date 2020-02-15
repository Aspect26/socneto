using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Domain.Services
{
    public class StorageService : IStorageService
    {
        private readonly HttpService<StorageService> _httpService;
        
        private IList<SocnetoComponent> _cachedAnalysers = new List<SocnetoComponent>();  

        public StorageService(IEventTracker<StorageService> eventTracker, IOptions<StorageOptions> storageOptionsObject)
        {
            if (string.IsNullOrEmpty(storageOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(storageOptionsObject.Value.ServerAddress));
            
            var host = storageOptionsObject.Value.ServerAddress;
            _httpService = new HttpService<StorageService>(host, eventTracker);
        }

        public async Task<bool> IsComponentRunning()
        {
            try
            {
                return await _httpService.GetString("health-check") != null;
            }
            catch (ServiceUnavailableException)
            {
                return false;
            }
        }

        public async Task<User> GetUser(string username)
        {
            try
            {
                return await _httpService.Get<User>($"users?username={username}");
            }
            catch (HttpRequestException)
            {
                // TODO: this is here only because storage returns 500 if the user is not found :(
                return null;
            }
        }

        public async Task<IList<Job>> GetUserJobs(string username)
        {
            return await _httpService.Get<List<Job>>($"jobs?username={username}");
        }

        public async Task<Job> GetJob(Guid jobId)
        {
            return await _httpService.Get<Job>($"jobs/{jobId}");
        }

        public async Task<JobView> GetJobView(Guid jobId)
        {
            return await _httpService.Get<JobView>($"jobs/{jobId}/view");
        }
        
        public async Task<JobView> StoreJobView(Guid jobId, JobView jobView)
        {
            return await _httpService.Post<JobView>($"jobs/{jobId}/view", jobView);
        }

        public async Task<JobView> UpdateJobView(Guid jobId, JobView jobView)
        {
            return await _httpService.Put<JobView>($"jobs/{jobId}/view", jobView);
        }

        public async Task<ListWithCount<Post>> GetPosts(Guid jobId, string[] allowedWords, 
            string[] forbiddenWords, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var body = new PostsStorageRequest
            {
                JobId = jobId,
                AllowedWords = allowedWords,
                ForbiddenWords = forbiddenWords,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };
            return await _httpService.Post<ListWithCount<Post>>($"analyzedPosts", body);
        }
        
        public async Task<IList<SocnetoComponent>> GetAnalysers()
        {
            var analysers = await _httpService.Get<List<SocnetoComponent>>($"components?type=DATA_ANALYSER");
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
            return await _httpService.Get<List<SocnetoComponent>>($"components?type=DATA_ACQUIRER");
        }

        public async Task<AggregationAnalysisResult> GetAnalysisAggregation(GetAggregationAnalysisStorageRequest getAnalysisRequest)
        {
            return await _httpService.Post<AggregationAnalysisResult>($"results", getAnalysisRequest);
        }

        public async Task<ArrayAnalysisResult> GetAnalysisArray(GetArrayAnalysisStorageRequest getAnalysisRequest)
        {
            return await _httpService.Post<ArrayAnalysisResult>($"results", getAnalysisRequest);
        }

        public async Task<TimeArrayAnalysisResult> GetAnalysisTimeArray(GetArrayAnalysisStorageRequest getAnalysisRequest)
        {
            return await _httpService.Post<TimeArrayAnalysisResult>($"results", getAnalysisRequest);
        }
    }
}
