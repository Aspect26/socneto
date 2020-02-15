using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisPropertyType
    {
        [EnumMember(Value = "numberValue")]
        Number,
        
        [EnumMember(Value = "textValue")]
        String,
        
        [EnumMember(Value = "numberListValue")]
        NumberList,
        
        [EnumMember(Value = "textListValue")]
        StringList,
        
        [EnumMember(Value = "numberMapValue")]
        NumberMap,
        
        [EnumMember(Value = "textMapValue")]
        StringMap
    }

    public class AnalyserDto
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        
        [JsonProperty("analysis_properties")]
        public Dictionary<string, AnalysisPropertyType> AnalysisProperties { get; set; }
        
        [JsonProperty("component_type")]
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
