using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ArrayAnalysisResponse
    {
        [JsonProperty("data")]
        public List<JArray> Result { get; set; }  
        
        public static ArrayAnalysisResponse FromModel(ArrayAnalysisResult analysisResult)
        {
            return new ArrayAnalysisResponse
            {
                Result = analysisResult.Result
            };
        }

        public static ArrayAnalysisResponse Empty()
        {
            return new ArrayAnalysisResponse
            {
                Result = new List<JArray>()
            };
        }
    }
}