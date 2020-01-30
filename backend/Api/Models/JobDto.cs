using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobDto
    {
        public Guid JobId { get; set; }
        
        public string JobName { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public  JobStatusCode Status { get; set; }
        
        public DateTime StartedAt { get; set; }
        
        public DateTime? FinishedAt { get; set; }

        public static JobDto FromModel(Job job)
        {
            return new JobDto()
            {
                JobId = job.JobId,
                JobName =  job.JobName,
                Status = job.Status,
                StartedAt =  job.StartedAt,
                FinishedAt = job.FinishedAt,
            };
        }
    }
}