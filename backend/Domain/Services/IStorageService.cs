using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IStorageService
    {
        Task<User> GetUser(string username);

        Task<IList<JobStatus>> GetUserJobs(string username);

        Task<JobStatus> GetJob(Guid jobId);

        Task<IList<AnalyzedPost>> GetAnalyzedPosts(Guid jobId);

        Task<IList<SocnetoComponent>> GetAnalysers();

        Task<IList<SocnetoComponent>> GetAcquirers();

    }
}