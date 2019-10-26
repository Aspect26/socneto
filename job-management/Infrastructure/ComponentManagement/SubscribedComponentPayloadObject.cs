using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.ComponentManagement
{
    public class SubscribedComponentPayloadObject
    {
        [JsonProperty("id")]
        public string ComponentId { get; set; }

        [JsonProperty("type")]
        public string ComponentType { get; set; }

        [JsonProperty("inputChannelName")]
        public string InputChannelName { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, JObject> Attributes { get; set; }
    }
}
