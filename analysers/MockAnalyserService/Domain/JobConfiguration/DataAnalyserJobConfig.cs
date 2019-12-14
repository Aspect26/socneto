using System;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class DataAnalyzerJobConfig
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("outputChannelNames")]
        public string[] OutputChannelNames { get; set; }
    }
}