using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AggregationAnalysisResponse
    {
        [JsonProperty("aggregations")]
        public Dictionary<string, double> Result { get; set; }  
        
        public static AggregationAnalysisResponse FromModel(AggregationAnalysisResult analysisResult)
        {
            return new AggregationAnalysisResponse()
            {
                Result = analysisResult.MapResult
            };
        }
    }
}