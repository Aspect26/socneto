using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobSubmitRequest
    {
        [JsonProperty("job_name")]
        public string JobName { get; set; }
        
        [JsonProperty("topic_query")]
        public string TopicQuery { get; set; }
        
        [JsonProperty("selected_acquirers")]
        public string[] SelectedAcquirers { get; set; }
        
        [JsonProperty("selected_analysers")]
        public string[] SelectedAnalysers { get; set; }
        
        // TODO: here we use model from Domain no API :(
        [JsonProperty("language")]
        public Language Language { get; set; } 
        
        [JsonProperty("attributes")]
        public Dictionary<string, Dictionary<string, string>> Attributes { get; set; }
    }

}