using System.Collections.Generic;

namespace Socneto.Coordinator.Api.Models
{
    public class PostDto
    {
        public string Text { get; set; }

        public double Sentiment { get; set; }

        public List<string> Keywords { get; set; }
    }
}