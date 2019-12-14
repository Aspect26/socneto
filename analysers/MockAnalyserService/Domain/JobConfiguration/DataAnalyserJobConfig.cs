using System;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class DataAnalyzerJobConfig
    {
        [JsonProperty("jsonId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("jsonId")]
        public string OutputChannelName { get; set; }
    }
}