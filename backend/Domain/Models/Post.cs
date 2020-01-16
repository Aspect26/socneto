using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class Post
    {
        [JsonProperty("authorId")]
        public string AuthorId { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("postedAt")]
        public DateTime? PostedAt { get; set; }
    }
}