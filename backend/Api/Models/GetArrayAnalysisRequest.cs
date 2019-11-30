using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Api.Models
{
    public class GetArrayAnalysisRequest
    {
        [JsonProperty("analyserId")]
        public string AnalyserId { get; set; }
        
        [JsonProperty("analysisProperties")]
        public string[] AnalysisProperties { get; set; }
    }
    
}