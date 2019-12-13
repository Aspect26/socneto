using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public enum AnalysisPropertyType
    {
        Number, String, List, Tuple
    }

    public class AnalysisProperty
    {
        public string Name { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisPropertyType Type { get; set; }
    }
    
    public class AnalyserDto
    {
        public string Identifier { get; set; }
        
        public IList<AnalysisProperty> AnalysisProperties { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SocnetoComponentType ComponentType { get; set; }

        public static AnalyserDto FromModel(SocnetoComponent model)
        {
            var analysisProperties = model.Attributes?["analysis_properties"];
            var typedAnalysisProperties = analysisProperties?.ToObject<List<AnalysisProperty>>();
            
            return new AnalyserDto
            {
                Identifier = model.ComponentId,
                ComponentType = model.ComponentType,
                AnalysisProperties = typedAnalysisProperties
            };
        }
        
    }
}
