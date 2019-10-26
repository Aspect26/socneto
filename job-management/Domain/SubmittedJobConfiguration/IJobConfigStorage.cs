using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SubmittedJobConfiguration
{
    public interface IJobConfigStorage
    {
        Task InsertNewJobConfigAsync(JobConfig jobConfig);

        Task<JobConfig> GetJobConfigAsync(Guid jobId);

        Task UpdateJobConfig(JobConfig jobConfig);
    }
}
