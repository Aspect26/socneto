using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Api.Models.Requests
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
        
        [JsonProperty("attributes")]
        public Dictionary<string, Dictionary<string, string>> Attributes { get; set; }
    }

}