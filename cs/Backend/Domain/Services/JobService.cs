using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class JobService : IJobService 
    {
        public Task<IList<JobStatus>> GetJobStatuses(int userId)
        {
            var random = new Random(userId);

            var jobStatuses =  (IList<JobStatus>)Enumerable.Range(0, random.Next(5, 15))
                .Select(r =>
                {
                    var arr = new byte[16];
                    random.NextBytes(arr);
                    return new Guid(arr);
                })
                .Select(RandomDataGenerator.GetRandomJobStatusResponse)
                .ToList();

            return Task.FromResult(jobStatuses);
        }
    }
}