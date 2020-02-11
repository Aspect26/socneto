using System;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class PostDto
    {
        [JsonProperty("author_id")]
        public string AuthorId { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("original_text")]
        public string OriginalText { get; set; }

        [JsonProperty("posted_at")]
        public DateTime? PostedAt { get; set; }

        public static PostDto FromValue(Post post)
        {
            return new PostDto
            {
                AuthorId = post.AuthorId,
                Text = post.Text,
                PostedAt = post.PostedAt,
                OriginalText = post.OriginalText,
            };
        }
    }
}