using System;

namespace Socneto.Domain.Models
{
    public class JobDataRequest
    {
        public Guid JobId { get; set; }
        public string Query { get; set; }
        
    }
}