using System;

namespace Socneto.Coordinator.Api.Models
{
    public class JobStatusResponse
    {
        public Guid JobId { get; set; }
        public int UserId { get; set; }
        public bool HasFinished { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}