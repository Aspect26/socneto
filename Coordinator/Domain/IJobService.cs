using System.Threading.Tasks;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Domain
{
    public interface IJobService
    {
        Task<JobSubmitResult> SubmitJob(JobSubmitInput jobInput);
    }
}