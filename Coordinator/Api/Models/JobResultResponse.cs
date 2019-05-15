using System;
using System.Collections.Generic;

namespace Socneto.Coordinator.Api.Models
{
    public class JobResultResponse
    {
        public string InputQuery { get; set; }

        public Guid JobId { get; set; }



        public List<PostDto> Posts { get; set; }

    }
}