using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobManagementService
    {

        Task<bool> IsComponentRunning();
        
        Task<JobStatus> SubmitJob(JobSubmit jobSubmit, Dictionary<string, Dictionary<string, string>> credentials);

        Task<JobStatus> StopJob(Guid jobId);
        
    }
}