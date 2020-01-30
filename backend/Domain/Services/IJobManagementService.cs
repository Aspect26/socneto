using System;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobManagementService
    {

        Task<bool> ComponentRunning();
        
        Task<JobStatus> SubmitJob(JobSubmit jobSubmit);

        Task<JobStatus> StopJob(Guid jobId);
        
    }
}