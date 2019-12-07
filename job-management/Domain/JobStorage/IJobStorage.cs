using Domain.Models;
using System;
using System.Threading.Tasks;

namespace Domain.JobStorage
{
    public interface IJobStorage
    {
        Task InsertNewJobAsync(Job job);
        Task UpdateJobAsync(Job job);

        Task<Job> GetJobAsync(Guid jobId);


    }
}