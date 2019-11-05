using System;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class Job
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("jobName")]
        public string JobName { get; set; }

        [JsonProperty("username")]
        public string Owner { get; set; }
        
        [JsonProperty("hasFinished")]
        public bool HasFinished { get; set; }
        
        [JsonProperty("startedAt")]
        public DateTime StartedAt { get; set; }
        
    }
    
}