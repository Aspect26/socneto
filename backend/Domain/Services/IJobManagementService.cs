using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobManagementService
    {
        Task<JobStatus> SubmitJob();

        Task<JobStatus> StopJob(string jobId);
        
    }
}