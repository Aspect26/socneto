using System;
using System.Collections.Generic;
using Domain.Acquisition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.JobConfiguration
{
    public enum JobCommand
    {
        Start, Stop
    }
    public class DataAcquirerJobConfig
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("command")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobCommand Command { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        [JsonProperty("outputMessageBrokerChannels")]
        public string[] OutputMessageBrokerChannels { get; set; }
    }

}
