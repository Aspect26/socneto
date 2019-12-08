using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Models
{
    public class RegistrationRequestMessage
    {
        [JsonProperty("componentType")]
        public string ComponentType { get; set; }

        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("inputChannelName")]
        public string InputChannelName { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }

        [JsonProperty("attributes")]
        public JObject Attributes { get; set; }

    }
}