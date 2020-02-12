using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class PostsStorageRequest
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("size")]
        public int PageSize { get; set; }
        
        [JsonProperty("allowedTerms")]
        public string[] AllowedWords { get; set; }
        
        [JsonProperty("forbiddenTerms")]
        public string[] ForbiddenWords { get; set; }
    }
}