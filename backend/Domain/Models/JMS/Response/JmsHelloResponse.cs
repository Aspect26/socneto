using Newtonsoft.Json;

namespace Socneto.Domain.Models.JMS.Response
{
    public class JmsHelloResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}