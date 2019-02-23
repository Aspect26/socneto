using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace SAnalyser.Api.Models
{
    public class TaskResponse
    {
        public string[] Keywords { get; set; }

        public static TaskResponse FromResult(TaskResult result)
        {
            return new TaskResponse()
            {
                Keywords = result.Keywords
            };
        }

    }
}
