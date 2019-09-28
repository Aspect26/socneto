using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobService
    {
        Task<IList<JobStatus>> GetJobStatuses(string username);
        Task<JobStatus> GetJobStatus(Guid jobId);
        Task<IList<Post>> GetJobPosts(Guid guid);
        Task<IList<AnalyzedPost>> GetJobAnalysis(Guid guid);
    }
}