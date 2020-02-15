using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public class AggregationAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        [JsonProperty("map")]
        public Dictionary<string, JToken> MapResult { get; set; }
    }
}
