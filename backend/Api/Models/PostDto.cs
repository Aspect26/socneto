using System;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class PostDto
    {
        public string AuthorId { get; set; }
        
        public string Text { get; set; }

        public DateTime PostedAt { get; set; }

        public static PostDto FromValue(Post post)
        {
            return new PostDto
            {
                AuthorId = post.AuthorId,
                Text = post.Text,
                PostedAt = post.PostedAt,
            };
        }
    }
}