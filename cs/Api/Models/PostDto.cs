using System;
using System.Collections.Generic;

namespace Socneto.Api.Models
{
    public class PostDto
    {
        public string Text { get; set; }

        public double Sentiment { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public List<string> Keywords { get; set; }
    }
}