using System.Collections.Generic;
using Newtonsoft.Json;

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
        public Dictionary<string,Dictionary<string,string>> Attributes { get; set; }

    }
}