using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public class AnalyzedPost
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("postDto")]
        public Post Post { get; set; }

        [JsonProperty("analyses")]
        public JObject Analyses { get; set; }
    }
}