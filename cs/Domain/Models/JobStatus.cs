using System;

namespace Socneto.Domain.Models
{
    public class JobStatus
    {
        public Guid JobId { get; set; }
        public int UserId { get; set; }
        public string JobName { get; set; }
        public bool HasFinished { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}