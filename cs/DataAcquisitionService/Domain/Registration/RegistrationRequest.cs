using Newtonsoft.Json;

namespace Domain.Registration
{
    public class RegistrationRequest
    {
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("componentType")]
        public string ComponentType { get; set; }
    }
}