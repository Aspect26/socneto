using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class AnalysisResponse
    {
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        [JsonProperty("postId")]
        public string PostId { get; set; }
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("analysis")]
        public Dictionary<string, AnalysisValue> Analysis { get; set; }

        public static AnalysisResponse FromData(string componentId, UniPost post, Analysis analysis)
        {
            return new AnalysisResponse
            {
                Analysis = analysis.Data,
                PostId = post.PostId,
                JobId = post.JobId,
                ComponentId = componentId
            };
        }
    }
}