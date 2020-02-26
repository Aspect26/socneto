using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;
using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Domain.Services
{
    public class JobService : IJobService
    {

        private readonly IStorageService _storageService;
        
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

        public async Task<Tuple<IList<Post>, int>> GetJobPosts(Guid jobId, string[] allowedWords, 
            string[] forbiddenWords, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var postsWithCount = await _storageService.GetPosts(jobId, allowedWords, forbiddenWords, fromDate, toDate, page - 1, pageSize);
            return new Tuple<IList<Post>, int>(postsWithCount.Data, postsWithCount.TotalCount);
        }
    }
}