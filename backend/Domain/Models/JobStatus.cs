using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class JobStatus
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("status")]
        public JobStatusCode Status { get; set; }
    }

}