using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;
using Socneto.Domain.Models.JMS.Response;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Api.Models.Responses
{
    public class JobDto
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }
        
        [JsonProperty("job_name")]
        public string JobName { get; set; }
        
        [JsonProperty("topic_query")]
        public string TopicQuery { get; set; }
        
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public  JobStatusCode Status { get; set; }
        
        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }
        
        [JsonProperty("finished_at")]
        public DateTime? FinishedAt { get; set; }

        public static JobDto FromModel(Job job)
        {
            return new JobDto()
            {
                JobId = job.JobId,
                JobName =  job.JobName,
                TopicQuery = job.TopicQuery,
                Status = job.Status,
                StartedAt =  job.StartedAt,
                FinishedAt = job.FinishedAt,
            };
        }
    }
}