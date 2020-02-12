using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Domain.JobStorage;
using Domain.Models;

namespace Infrastructure
{
    public class InMemoryJobStorage : IJobStorage
    {
        private readonly ConcurrentDictionary<Guid, Job> _jobs = new ConcurrentDictionary<Guid, Job>();
        public Task<Job> GetJobAsync(Guid jobId)
        {
            if(_jobs.TryGetValue(jobId,out var job))
            {
                return Task.FromResult(job);
            }
            return null;
        }

        public Task InsertNewJobAsync(Job job)
        {
            _jobs.TryAdd(job.JobId, job);
            return Task.CompletedTask;
        }

        public async Task UpdateJobAsync(Job job)
        {
            var previous = await GetJobAsync(job.JobId);
            _jobs.TryUpdate(job.JobId, job, previous);
        }
    }
}