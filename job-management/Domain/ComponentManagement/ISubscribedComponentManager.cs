using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;

namespace Domain.ComponentManagement
{
    public interface ISubscribedComponentManager
    {
        Task<SubscribedComponentResultModel> SubscribeComponentAsync(ComponentModel componentRegistrationModel);

        Task<JobConfigUpdateResult> StartJobAsync(JobConfigUpdateCommand jobConfigUpdateCommand);

        Task<JobConfigUpdateResult> StopJob(Guid jobId);
    }
}
