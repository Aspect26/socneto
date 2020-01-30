using System;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobStatusResponse
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }
        
        [JsonProperty("status")]
        public JobStatusCode Status { get; set; }
    }

}