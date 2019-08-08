using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class Post
    {
        public string AuthorId { get; set; }
        
        public string Text { get; set; }

        // TODO: remove this when we return actual datetime object
        [JsonIgnore]
        public DateTime PostedAt { get; set; }
    }
}