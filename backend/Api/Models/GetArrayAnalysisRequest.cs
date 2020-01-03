using Newtonsoft.Json;

namespace Socneto.Api.Models
{
    public class GetArrayAnalysisRequest
    {
        [JsonProperty("analyser_id")]
        public string AnalyserId { get; set; }
        
        [JsonProperty("analysis_properties")]
        public string[] AnalysisProperties { get; set; }
    }
    
}