using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class Job
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public string Username { get; set; }
        public string TopicQuery { get; set; }
        public JobStatusCode Status { get; set; }
        public string Language { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}