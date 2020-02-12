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
        
        Task<Tuple<IList<Post>, int>> GetJobPosts(Guid jobId, string[] allowedWords, string[] forbiddenWords, 
            int page, int pageSize);

        Task<IList<AnalyzedPost>> GetAllJobPosts(Guid jobId, string[] allowedWords, string[] forbiddenWords);
    }
}