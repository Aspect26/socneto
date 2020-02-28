using System;
using Newtonsoft.Json;
using Socneto.Domain.Models;
using Socneto.Domain.Models.JMS.Response;

namespace Socneto.Api.Models.Responses
{
    public class JobStatusResponse
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }
        
        [JsonProperty("status")]
        public JobStatusCode Status { get; set; }
    }

}