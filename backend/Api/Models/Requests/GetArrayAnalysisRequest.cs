using Newtonsoft.Json;

namespace Socneto.Api.Models.Requests
{
    public class GetArrayAnalysisRequest
    {
        [JsonProperty("analyser_id")]
        public string AnalyserId { get; set; }
        
        [JsonProperty("analysis_properties")]
        public string[] AnalysisProperties { get; set; }
        
        [JsonProperty("is_x_post_date")]
        public bool IsXPostDate { get; set; }

        [JsonProperty("page_size")] 
        public int PageSize { get; set; } = 200;

        [JsonProperty("page")] 
        public int Page { get; set; } = 1;
    }
    
}