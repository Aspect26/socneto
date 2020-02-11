using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobService
    {
        Task<IList<Job>> GetJobsDetails(string username);
        
        Task<Job> GetJobDetail(Guid jobId);
        
        Task<Tuple<IList<AnalyzedPost>, int>> GetJobPosts(Guid jobId, int offset, int pageSize);
        
        Task<IList<AnalyzedPost>> GetAllJobPosts(Guid jobId);
    }
}