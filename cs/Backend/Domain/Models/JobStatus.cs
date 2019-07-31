using System;

namespace Socneto.Domain.Models
{
    // TODO: change me to Job model
    public class JobStatus
    {
        public Guid JobId { get; set; }
        public string Username { get; set; }
        public string JobName { get; set; }
        public bool HasFinished { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}