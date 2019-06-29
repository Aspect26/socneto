using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain
{
    public interface IJobService
    {
        Task<JobSubmitResult> SubmitJob(JobSubmitInput jobInput);
    }
}