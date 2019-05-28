using System;

namespace Socneto.Domain.Models
{
    public class JobSubmitResult
    {
        public Guid JobId { get; set; }
    }
    public class TaskInput
    {
        public string Query { get; set; }
    }
}