﻿using System.Linq;
using System.Threading.Tasks;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Api.Models
{
    public class JobRequest
    {
        public string Query { get; set; }

        //public string FromDate { get; set; }

        //public string ToDate { get; set; }

        public static JobSubmitInput ToTaskInput(JobRequest taskRequest)
        {
            return  new JobSubmitInput(){Query =  taskRequest.Query};
        }
    }
}
