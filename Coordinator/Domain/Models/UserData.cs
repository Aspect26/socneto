
using System;

namespace Socneto.Coordinator.Domain.Models
{
    public class UserData
    {
        public string Name { get; set; }
    }

    public class JobDataRequest
    {
        public Guid JobId { get; set; }
        public string Query { get; set; }
        
    }
}