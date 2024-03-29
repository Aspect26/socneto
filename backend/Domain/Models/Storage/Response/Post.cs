﻿using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models.Storage.Response
{
    public class Post
    {
        [JsonProperty("id")]
        public Guid PostId { get; set; }
        
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("originalId")]
        public string OriginalId { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("originalText")]
        public string OriginalText { get; set; }
        
        [JsonProperty("authorId")]
        public string AuthorId { get; set; }
        
        [JsonProperty("language")]
        public string Language { get; set; }
        
        [JsonProperty("datetime")]
        public DateTime? PostedAt { get; set; }
    }
}