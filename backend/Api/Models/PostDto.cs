using System;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class PostDto
    {
        [JsonProperty("original_id")]
        public string OriginalId { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("original_text")]
        public string OriginalText { get; set; }

        [JsonProperty("posted_at")]
        public DateTime? PostedAt { get; set; }

        public static PostDto FromModel(Post post)
        {
            return new PostDto
            {
                OriginalId = post.OriginalId,
                Text = post.Text,
                OriginalText = post.OriginalText,
                PostedAt = post.PostedAt,
            };
        }
    }
}