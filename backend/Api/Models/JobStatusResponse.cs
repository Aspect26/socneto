using System;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobStatusResponse
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public bool HasFinished { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public static JobStatusResponse FromModel(JobStatus jobStatus)
        {
            return new JobStatusResponse()
            {
                JobId = jobStatus.JobId,
                JobName =  jobStatus.JobName,
                HasFinished = jobStatus.HasFinished,
                StartedAt =  jobStatus.StartedAt,
                FinishedAt = jobStatus.FinishedAt
            };
        }
    }
}