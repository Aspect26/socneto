using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class DataAcquisitionConfigUpdateNotification
    {

        //TODO add
        //jobid

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        [JsonProperty("outputMessageBrokerChannel")]
        public string OutputMessageBrokerChannel { get; set; }
    }
}