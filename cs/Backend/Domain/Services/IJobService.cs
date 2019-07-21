using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobService
    {
        Task<JobSubmitResult> SubmitJob(JobSubmitInput jobInput);
    }
}