using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.JMS.Response;

namespace Socneto.Domain.Services
{
    public interface IJobManagementService
    {

        Task<bool> IsComponentRunning();
        
        Task<JobStatus> SubmitJob(JobSubmit jobSubmit, Dictionary<string, Dictionary<string, string>> originalAttributes);

        Task<JobStatus> StopJob(Guid jobId);
        
    }
}