using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Models
{
    public class DataAcquisitionConfigUpdateNotification
    {
        [JsonProperty("jobId")]
        public Guid  JobId { get; set; }
        
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, JObject> Attributes { get; set; }

        [JsonProperty("outputMessageBrokerChannels")]
        public string[]  OutputMessageBrokerChannels { get; set; }
    }
}