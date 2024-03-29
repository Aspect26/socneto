﻿using System;
using Domain.SubmittedJobConfiguration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Domain.Models
{
    public class DataAcquisitionConfigUpdateNotification
    {
        [JsonProperty("jobId")]
        public Guid  JobId { get; set; }
        
        [JsonProperty("command")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobCommand Command { get; set; }

        [JsonProperty("attributes")]
        public JObject Attributes { get; set; }

        [JsonProperty("outputChannelNames")]
        public string[]  OutputMessageBrokerChannels { get; set; }
    }
}