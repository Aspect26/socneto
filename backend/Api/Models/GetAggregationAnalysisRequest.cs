using Newtonsoft.Json;

namespace Socneto.Api.Models
{
    public class GetAggregationAnalysisRequest
    {
        [JsonProperty("analyserId")]
        public string AnalyserId { get; set; }
        
        [JsonProperty("analysisProperty")]
        public string AnalysisProperty { get; set; }
    }
}