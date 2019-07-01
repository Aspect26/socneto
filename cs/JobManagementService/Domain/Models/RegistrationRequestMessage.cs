using Newtonsoft.Json;

namespace Domain.Models
{
    public class RegistrationRequestMessage
    {
        [JsonProperty("componentType")]
        public string ComponentType { get; set; }
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("updateChannelName")]
        public string UpdateChannelName { get; set; }

    }
}