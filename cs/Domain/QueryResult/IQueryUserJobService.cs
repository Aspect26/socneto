using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.QueryResult
{
    public interface IQueryUserJobService
    {
        Task<IList<JobStatus>> GetJobStatuses(int userId);
    }
}