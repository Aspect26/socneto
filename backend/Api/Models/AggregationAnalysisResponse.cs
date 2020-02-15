using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AggregationAnalysisResponse
    {
        [JsonProperty("aggregations")]
        public Dictionary<string, JToken> Result { get; set; }  
        
        public static AggregationAnalysisResponse FromModel(AggregationAnalysisResult analysisResult)
        {
            return new AggregationAnalysisResponse
            {
                Result = analysisResult.MapResult
            };
        }

        public static AggregationAnalysisResponse Empty()
        {
            return new AggregationAnalysisResponse
            {
                Result = new Dictionary<string, JToken>()
            };
        }
    }
}