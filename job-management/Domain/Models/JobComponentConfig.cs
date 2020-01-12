using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Domain.Models
{
    public class JobComponentConfig
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("attributes")]
        public JObject Attributes { get; set; }

        [JsonProperty("outputChannelNames")]
        public string[] OutputMessageBrokerChannels { get; set; }
    }

}