using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AnalyzedPostDto
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        [JsonProperty("post")]
        public PostDto PostDto { get; set; }
        
        [JsonProperty("analyses")]
        public JArray Analyses { get; set; }

        public static AnalyzedPostDto FromModel(AnalyzedPost post)
        {
            return new AnalyzedPostDto
            {
                JobId =  post.JobId,
                PostDto = PostDto.FromModel(post.Post),
                Analyses = post.Analyses,
            };
        }
    }
}