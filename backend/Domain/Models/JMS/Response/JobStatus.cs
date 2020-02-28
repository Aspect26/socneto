using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models.JMS.Response
{
    public class JobStatus
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("status")]
        public JobStatusCode Status { get; set; }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobStatusCode
    {
        
        [EnumMember(Value = "Running")]
        Running,
        
        [EnumMember(Value = "Stopped")]
        Stopped,
    }

}