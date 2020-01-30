using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Api.Models
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
}