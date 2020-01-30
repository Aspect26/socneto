using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class JMSHello
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}