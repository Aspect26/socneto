using System;
using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class JobResult
    {
        public string InputQuery { get; set; }

        public Guid JobId { get; set; }

        
        public List<Post> Posts { get; set; }
    }
}