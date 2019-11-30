using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ArrayAnalysisResponse
    {
        // TODO: object :(
        [JsonProperty("data")]
        public List<List<object>> Result { get; set; }  
        
        public static ArrayAnalysisResponse FromModel(ArrayAnalysisResult analysisResult)
        {
            return new ArrayAnalysisResponse()
            {
                Result = analysisResult.Result
            };
        }
    }
}