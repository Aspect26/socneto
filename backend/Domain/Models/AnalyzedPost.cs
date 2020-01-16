using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class AnalyzedPost
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("postDto")]
        public Post Post { get; set; }

        [JsonProperty("analyses")]
        public Dictionary<string,  Dictionary<string, AnalysisValue>>[] Analyses { get; set; }
    }
}