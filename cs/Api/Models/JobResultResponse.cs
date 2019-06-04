using System;
using System.Collections.Generic;
using System.Linq;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobResultResponse
    {
        public string InputQuery { get; set; }

        public Guid JobId { get; set; }



        public List<PostDto> Posts { get; set; }

        public static JobResultResponse FromModel(JobResult jobResult)
        {
            return new JobResultResponse()
            {
                JobId = jobResult.JobId,
                InputQuery = jobResult.InputQuery,
                Posts = jobResult.Posts
                    .Select(PostDto.FromValue)
                    .ToList()
            };
        }
    }
}