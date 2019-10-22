using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class Job
    {
        public Guid JobId { get; set; }
        public string Username { get; set; }
        public string JobName { get; set; }
        public bool IsRunning { get; set; }
        
        // TODO: remove these ignores when datetimes are sent as datetime, not longs
        [JsonIgnore]
        public DateTime StartedAt { get; set; }
        [JsonIgnore]
        public DateTime? PausedAt { get; set; }
    }
}