using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAnalyser.Api.Models
{
    public class TaskRequest
    {
        public string Topic { get; set; }
        public string FromDate { get; set; }

        public string ToDate { get; set; }

    }
}
