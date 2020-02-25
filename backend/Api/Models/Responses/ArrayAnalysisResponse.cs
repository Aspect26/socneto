using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Api.Models.Responses
{
    public class ArrayAnalysisResponse
    {
        [JsonProperty("data")]
        public List<JArray> Result { get; set; }  
        
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        
        public static ArrayAnalysisResponse FromModel(ArrayAnalysisResult analysisResult)
        {
            return new ArrayAnalysisResponse
            {
                Result = analysisResult.Result,
                TotalCount = analysisResult.TotalCount
            };
        }

        public static ArrayAnalysisResponse Empty()
        {
            return new ArrayAnalysisResponse
            {
                Result = new List<JArray>(),
                TotalCount = 0
            };
        }
    }
}