﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class AnalyserConfigUpdateNotification
    {
        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }
        [JsonProperty("inputMessageBrokerChannel")]
        public string InputMessageBrokerChannel { get; set; }
        [JsonProperty("outputMessageBrokerChannel")]
        public string OutputMessageBrokerChannel { get; set; }
    }
}