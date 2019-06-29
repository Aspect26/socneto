using System;
using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class PostDto
    {
        public string Text { get; set; }

        public double Sentiment { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public List<string> Keywords { get; set; }

        public static PostDto FromValue(Post post)
        {
            return new PostDto
            {
                UserId =  post.UserId,
                DateTime = post.DateTime,
                Keywords = post.Keywords,
                Sentiment =  post.Sentiment,
                Text = post.Text
            };
        }
    }
}