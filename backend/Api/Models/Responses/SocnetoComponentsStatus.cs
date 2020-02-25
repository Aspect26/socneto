using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Api.Models.Responses
{
    public class SocnetoComponentsStatus
    {
        [JsonProperty("jms")]
        public SocnetoComponentStatus JmsStatus { get; set; }
        
        [JsonProperty("storage")]
        public SocnetoComponentStatus StorageStatus { get; set; }
        
        [JsonProperty("acquirers")]
        public Dictionary<string, SocnetoComponentStatus> AcquirersStatus { get; set; }
        
        [JsonProperty("analysers")]
        public Dictionary<string, SocnetoComponentStatus> AnalysersStatus { get; set; }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocnetoComponentStatus
    {
        [EnumMember(Value = "STOPPED")]
        Stopped,
        
        [EnumMember(Value = "RUNNING")]
        Running
    }
}