using System;
using Newtonsoft.Json;
using Socneto.Domain.Helpers;

namespace Socneto.Domain.Models
{
    public class Post
    {
        public string AuthorId { get; set; }
        
        public string Text { get; set; }

        [JsonConverter(typeof(JsonMillisecondsToDateTimeConverter))]
        public DateTime PostedAt { get; set; }
    }
}