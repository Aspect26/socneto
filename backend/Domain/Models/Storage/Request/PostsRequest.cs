using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models.Storage.Request
{
    public class PostsRequest
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
        
        [JsonProperty("fromDate")]
        public DateTime? FromDate { get; set; }
        
        [JsonProperty("toDate")]
        public DateTime? ToDate { get; set; }
    }
}