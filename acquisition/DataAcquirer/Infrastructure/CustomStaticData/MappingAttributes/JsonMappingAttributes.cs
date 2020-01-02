using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData.MappingAttributes
{
    public class JsonMappingAttributes
    {
        [JsonProperty("hasHeaders")]
        public bool HasHeaders { get; set; }

        [JsonProperty("dateTimeFormatString")]
        public string DateTimeFormatString { get; set; }

        [JsonProperty("fixedValues")]
        public Dictionary<string, string> FixedValues { get; set; }

        [JsonProperty("elements")]
        public Dictionary<string, string> Elements { get; set; }
    }
}
