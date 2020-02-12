using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Domain.Services
{
    public interface IStorageService
    {
        Task<bool> IsComponentRunning();
        
        Task<User> GetUser(string username);

        Task<IList<Job>> GetUserJobs(string username);

        Task<Job> GetJob(Guid jobId);

        Task<ListWithCount<Post>> GetPosts(Guid jobId, string[] allowedWords, string[] forbiddenWords, int page, int pageSize);
        
        Task<IList<AnalyzedPost>> GetAllPosts(Guid jobId, string[] allowedWords, string[] forbiddenWords);

        Task<JobView> GetJobView(Guid jobId);

        Task<JobView> StoreJobView(Guid jobId, JobView jobView);

        Task<JobView> UpdateJobView(Guid jobId, JobView jobView);

        Task<IList<SocnetoComponent>> GetAnalysers();

        Task<SocnetoComponent> GetAnalyser(string identifier);

        Task<IList<SocnetoComponent>> GetAcquirers();

        Task<AggregationAnalysisResult> GetAnalysisAggregation(GetAggregationAnalysisStorageRequest getAnalysisRequest);

        Task<ArrayAnalysisResult> GetAnalysisArray(GetArrayAnalysisStorageRequest getAnalysisStorageRequest);
    }
}