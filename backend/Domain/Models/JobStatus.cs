using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    // TODO: change my name to Job model
    public class JobStatus
    {
        public Guid JobId { get; set; }
        public string Username { get; set; }
        public string JobName { get; set; }
        public bool HasFinished { get; set; }
        
        // TODO: remove these ignores when datetimes are sent as datetime, not longs
        [JsonIgnore]
        public DateTime StartedAt { get; set; }
        [JsonIgnore]
        public DateTime? FinishedAt { get; set; }
    }
}