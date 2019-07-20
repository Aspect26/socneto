using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IStorageService
    {
        Task<User> GetUser(int userId);

        Task<List<JobStatus>> GetUserJobs(int userId);

        Task<JobStatus> GetJob(string jobId);

        Task<List<Post>> GetAnalyzedPosts(string jobId);
        
    }
}