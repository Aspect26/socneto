using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class AggregationAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        [JsonProperty("map")]
        public Dictionary<string, double> MapResult { get; set; }
    }
}