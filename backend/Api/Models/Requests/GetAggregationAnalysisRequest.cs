using Newtonsoft.Json;

namespace Socneto.Api.Models.Requests
{
    public class GetAggregationAnalysisRequest
    {
        [JsonProperty("analyser_id")]
        public string AnalyserId { get; set; }
        
        [JsonProperty("analysis_property")]
        public string AnalysisProperty { get; set; }
    }
}