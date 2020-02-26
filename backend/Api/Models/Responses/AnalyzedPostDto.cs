using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Models;

namespace Socneto.Api.Models.Responses
{
    public class AnalyzedPostDto
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        [JsonProperty("post")]
        public PostDto PostDto { get; set; }
        
        [JsonProperty("analyses")]
        public JArray Analyses { get; set; }

    }
}