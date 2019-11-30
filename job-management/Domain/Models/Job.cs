using System;
using System.Collections.Generic;
using Domain.SubmittedJobConfiguration;
using Newtonsoft.Json;

namespace Domain.Models
{

    public class Job
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("jobName")]
        public string JobName { get; set; }

        [JsonProperty("username")]
        public string Owner { get; set; }

        [JsonProperty("finished")]
        public long? FinishedAt { get; set; }

        [JsonProperty("startedAt")]
        public long StartedAt { get; set; }

        [JsonProperty("topicQuery")]
        public string TopicQuery { get; set; }

        [JsonProperty("status")]
        public JobStatus JobStatus { get; set; }

        [JsonProperty("componentConfigs")]
        public List<JobComponentConfig> JobComponentConfigs { get; set; }
    }

    

}