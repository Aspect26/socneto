using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public class JobSubmit
    {
        [JsonProperty("jobName")]
        public string JobName { get; set; }
        
        [JsonProperty("topicQuery")]
        public string TopicQuery { get; set; }

        [JsonProperty("selectedDataAcquirers")]
        public string[] SelectedAcquirersIdentifiers;
        
        [JsonProperty("selectedDataAnalysers")]
        public string[] SelectedAnalysersIdentifiers;

        [JsonProperty("attributes")] 
        public JObject Attributes;
    }
    
}