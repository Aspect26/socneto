using System;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobDto
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public bool IsRunning { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? PausedAt { get; set; }

        public static JobDto FromModel(Job job)
        {
            return new JobDto()
            {
                JobId = job.JobId,
                JobName =  job.JobName,
                IsRunning = job.IsRunning,
                StartedAt =  job.StartedAt,
                PausedAt = job.PausedAt
            };
        }
    }
}