using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class JobComponentConfig
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("type")]
        public string ComponentType { get; set; }

        [JsonProperty("inputChannelName")]
        public string InputChannelName { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string,object> Attributes { get; set; }
    }

}