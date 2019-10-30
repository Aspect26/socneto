using System;
using System.Security.AccessControl;
using Domain.SubmittedJobConfiguration;
using Newtonsoft.Json;

namespace Api.Models
{
    public class JobResponse
    {
        [JsonProperty("status")]
        public string Status { get; }

        [JsonProperty("jobId")]
        public Guid? JobId { get; }

        
        public JobResponse(Guid jobId, JobStatus status)
        {
            JobId = jobId;
            Status = status.ToString().ToLower();
        }
    }
}