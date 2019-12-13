using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Models
{
    public class JobSubmitRequest
    {
        [JsonProperty("selectedDataAnalysers")]
        public List<string> SelectedDataAnalysers { get; set; }

        [JsonProperty("selectedDataAcquirers")]
        public List<string> SelectedDataAcquirers { get; set; }

        [JsonProperty("topicQuery")]
        public string TopicQuery { get; set; }
        
        [JsonProperty("jobName")]
        public string JobName { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("attributes")]

        public JObject Attributes { get; set; }
    }
}