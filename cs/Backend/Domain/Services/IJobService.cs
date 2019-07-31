using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IJobService
    {
        Task<IList<JobStatus>> GetJobStatuses(int userId);
    }
}