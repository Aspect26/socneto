using Newtonsoft.Json;

namespace Domain.Registration
{
    public class RegistrationRequest
    {
        [JsonProperty("componentType")]
        public string ComponentType { get; set; }
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }
    }
}