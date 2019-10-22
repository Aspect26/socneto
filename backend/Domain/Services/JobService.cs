using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using System.Linq;

namespace Socneto.Domain.Services
{
    public class JobService : IJobService
    {

        private IStorageService _storageService;
        
        public JobService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<Job> GetJobDetail(Guid jobId)
        {
            return await _storageService.GetJob(jobId);
        }

        public async Task<IList<Job>> GetJobsDetails(string username)
        {
            return await _storageService.GetUserJobs(username);
        }

        public async Task<IList<Post>> GetJobPosts(Guid jobId)
        {
            // TODO: better ask for new storage call specifically for posts only
            var analyzedPosts = await _storageService.GetAnalyzedPosts(jobId);
            return analyzedPosts.Select(analyzedPost => analyzedPost.Post).ToList();
        }

        public async Task<IList<AnalyzedPost>> GetJobAnalysis(Guid jobId)
        {
            return await _storageService.GetAnalyzedPosts(jobId);
        }
    }
}