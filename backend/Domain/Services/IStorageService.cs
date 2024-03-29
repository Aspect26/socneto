using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Request;
using Socneto.Domain.Models.Storage.Response;
using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Domain.Services
{
    public interface IStorageService
    {
        Task<bool> IsComponentRunning();
        
        Task<User> GetUser(string username);

        Task<IList<Job>> GetUserJobs(string username);

        Task<Job> GetJob(Guid jobId);

        Task<ListWithCount<Post>> GetPosts(Guid jobId, string[] allowedWords, string[] forbiddenWords, DateTime? fromDate, 
            DateTime? toDate, int page, int pageSize);
        
        Task<JobView> GetJobView(Guid jobId);

        Task<JobView> StoreJobView(Guid jobId, JobView jobView);

        Task<JobView> UpdateJobView(Guid jobId, JobView jobView);

        Task<IList<SocnetoComponent>> GetAnalysers();

        Task<SocnetoComponent> GetAnalyser(string identifier);

        Task<IList<SocnetoComponent>> GetAcquirers();

        Task<AggregationAnalysisResult> GetFrequencyAggregation(GetFrequencyAnalysisStorageRequest getAnalysisRequest);
        
        Task<AggregationAnalysisResult> GetAnalysisAggregation(GetAggregationAnalysisStorageRequest getAnalysisRequest);

        Task<ArrayAnalysisResult> GetAnalysisArray(GetArrayAnalysisStorageRequest getAnalysisStorageRequest);
        
        Task<TimeArrayAnalysisResult> GetAnalysisTimeArray(GetArrayAnalysisStorageRequest getAnalysisStorageRequest);
    }
}