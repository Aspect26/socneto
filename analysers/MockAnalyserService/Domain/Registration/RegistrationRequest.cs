using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Registration
{
    public class RegistrationRequest
    {
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("componentType")]
        public string ComponentType { get; set; }

        [JsonProperty("inputChannelName")]
        public string InputChannelName { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }
        
        [JsonProperty("attributes")]
        public Dictionary<string, Dictionary<string, string>> Attributes { get; set; }
    }
}