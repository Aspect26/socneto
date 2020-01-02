using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.CustomStaticData.MappingAttributes
{
    public class MappingAttributesRoot
    {

        [JsonProperty("dataFormat")]
        public string DataFormat { get; set; }

        [JsonProperty("mappingAttributes")]
        public JObject MappingAttributes { get; set; }
    }
}
