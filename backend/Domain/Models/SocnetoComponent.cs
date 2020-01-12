using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public enum SocnetoComponentType
    {
        DATA_ANALYSER,
        DATA_ACQUIRER
    }
    
    public class SocnetoComponent
    {
        
        public string ComponentId { get; set; }
        
        public SocnetoComponentType ComponentType { get; set; }
        
        // public string InputChannelName { get; set; }
        
        // public string UpdateChannelName { get; set; }

        public JObject Attributes { get; set; }
        
    }
}
