using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AcquirerDto
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        
        [JsonProperty("component_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public  SocnetoComponentType ComponentType { get; set; }
        
        public static AcquirerDto FromModel(SocnetoComponent model)
        {
            return new AcquirerDto
            {
                Identifier = model.ComponentId,
                ComponentType = model.ComponentType,
            };
        }
        
    }
}
