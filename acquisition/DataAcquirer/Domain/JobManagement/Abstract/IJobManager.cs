using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.JobConfiguration;

namespace Domain.JobManagement.Abstract
{
    public interface IJobManager
    {
        Task StartNewJobAsync(DataAcquirerJobConfig jobConfig);
        Task StopJobAsync(Guid jobId);

        Task StopAllJobsAsync();
    }
}
