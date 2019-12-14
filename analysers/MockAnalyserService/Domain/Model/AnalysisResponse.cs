using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class AnalysisResponse
    {
        [JsonProperty("postId")]
        public Guid PostId { get; set; }

        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("componentId")]
        public string ComponentId { get; set; }

        [JsonProperty("results")]
        public Dictionary<string, AnalysisResult> Results { get; set; }

        // TODO: use the dictionary directly instead of Analysis object
        public static AnalysisResponse FromData(string componentId, UniPost post, Analysis analysis)
        {
            return new AnalysisResponse
            {
                Results = analysis.Data,
                PostId = post.PostId,
                JobId = post.JobId,
                ComponentId = componentId
            };
        }
    }
}