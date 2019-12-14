using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisPropertyType
    {
        Number, String, List, Tuple
    }

    public class AnalyserDto
    {
        public string Identifier { get; set; }
        
        public Dictionary<string, AnalysisPropertyType> AnalysisProperties { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SocnetoComponentType ComponentType { get; set; }

        public static AnalyserDto FromModel(SocnetoComponent model)
        {
            var analysisProperties = model.Attributes?["outputFormat"];
            var typedAnalysisProperties = analysisProperties?.ToObject<Dictionary<string, AnalysisPropertyType>>();
            
            return new AnalyserDto
            {
                Identifier = model.ComponentId,
                ComponentType = model.ComponentType,
                AnalysisProperties = typedAnalysisProperties
            };
        }
        
    }
}
