using System;

namespace Socneto.Domain.Models
{
    public class Post
    {
        [Obsolete("Is this supposed to be here?")]
        public int Id { get; set; }
        
        public string AuthorId { get; set; }
        
        public string Text { get; set; }

        public DateTime PostedAt { get; set; }
    }
}