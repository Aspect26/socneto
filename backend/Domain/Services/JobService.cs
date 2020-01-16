using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

using DataPoint = System.Collections.Generic.IList<dynamic>;


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

        public async Task<Tuple<IList<AnalyzedPost>, int>> GetJobPosts(Guid jobId, int offset, int size)
        {
            var posts = await _storageService.GetAnalyzedPosts(jobId, offset, size);
            // TODO: storage service should support paginating
            var actualSize = Math.Min(size, posts.Count - offset);
            var pagePosts = ((List<AnalyzedPost>) posts).GetRange(offset, actualSize);

            return new Tuple<IList<AnalyzedPost>, int>(pagePosts, posts.Count);
        }
    }
}