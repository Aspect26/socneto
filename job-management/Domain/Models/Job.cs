using System;
using System.Collections.Generic;
using Domain.SubmittedJobConfiguration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public DateTime? FinishedAt { get; set; }

        [JsonProperty("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonProperty("topicQuery")]
        public string TopicQuery { get; set; }


        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobStatus JobStatus { get; set; }
    }
}