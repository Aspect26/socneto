using System;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public class LogMessage
    {
        
        public string ComponentId { get; set; }
        
        public string EventType { get; set; } // FATAL, ERROR, WARN, INFO, METRIC
        
        public string EventName { get; set; }
        
        public string Message { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public  JObject Attributes { get; set; }
        
    }
}