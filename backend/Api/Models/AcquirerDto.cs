using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AcquirerDto
    {
        public string Identifier { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public  SocnetoComponentType ComponentType { get; set; }
        
        public static AcquirerDto FromModel(SocnetoComponent model)
        {
            return new AcquirerDto
            {
                Identifier = model.Id,
                ComponentType = model.SocnetoComponentType,
            };
        }
        
    }
}
