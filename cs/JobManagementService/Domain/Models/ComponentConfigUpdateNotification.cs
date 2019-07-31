using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class AnalyserConfigUpdateNotification
    {
        [JsonProperty("jsonId")]
        public Guid JobId { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }
        
        [JsonProperty("outputMessageBrokerChannels")]
        public string[] OutputMessageBrokerChannels { get; set; }
    }
}