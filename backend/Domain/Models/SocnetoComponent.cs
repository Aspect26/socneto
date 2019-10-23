using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public enum SocnetoComponentType
    {
        DataAnalyser,
        DataAcquirer
    }
    
    public class SocnetoComponent
    {
        
        public string Id { get; set; }
        
        public SocnetoComponentType SocnetoComponentType { get; set; }
        
        public string InputChannelName { get; set; }
        
        public string UpdateChannelName { get; set; }

        public JObject Attributes { get; set; }
        
    }
}
