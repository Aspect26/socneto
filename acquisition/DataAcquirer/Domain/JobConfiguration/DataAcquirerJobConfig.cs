using System;
using System.Collections.Generic;
using Domain.Acquisition;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class DataAcquirerJobConfig
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        [JsonProperty("outputMessageBrokerChannels")]
        public string[] OutputMessageBrokerChannels { get; set; }
    }

}
