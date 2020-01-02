using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData.MappingAttributes
{
    public class CsvMappingAttributes
    {
        [JsonProperty("hasHeaders")]
        public bool HasHeaders { get; set; }

        [JsonProperty("dateTimeFormatString")]
        public string DateTimeFormatString { get; set; }

        [JsonProperty("fixedValues")]
        public Dictionary<string, string> FixedValues { get; set; }

        [JsonProperty("indices")]
        public Dictionary<string, int> Indices { get; set; }
    }
}
