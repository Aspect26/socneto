using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocnetoComponentType
    {
        [EnumMember(Value = "DATA_ANALYSER")]
        DataAnalyser,
        
        [EnumMember(Value = "DATA_ACQUIRER")]
        DataAcquirer
    }
    
    public class SocnetoComponent
    {
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("type")]
        public SocnetoComponentType ComponentType { get; set; }
        
        [JsonProperty("attributes")]
        public JObject Attributes { get; set; }
        
    }
    
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
}
