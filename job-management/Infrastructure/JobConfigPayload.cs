using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class JobConfigPayload
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("dataAnalysers")]
        public List<string> DataAnalysers { get; set; }

        [JsonProperty("dataAcquirers")]
        public List<string> DataAcquirers { get; set; }

        [JsonProperty("topicQuery")]
        public string TopicQuery { get; set; }

        [JsonProperty("status")]
        public string JobStatus { get; set; }
    }
}