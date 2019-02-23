using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class TaskInput
    {
        public string Topic { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
