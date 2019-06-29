using System;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.QueryResult
{
    public interface IQueryJobResultService
    {
        Task<JobStatus > GetJobStatus(Guid jobId);
        Task<JobResult> GetJobResult(Guid jobId);
    }

    
}