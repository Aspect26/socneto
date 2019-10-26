using System;
using System.Collections.Generic;

namespace Domain.SubmittedJobConfiguration
{
    public class JobConfig
    {
        public Guid JobId { get; set; }
        public List<string> DataAnalysers { get; set; }
        public List<string> DataAcquirers { get; set; }
        public string TopicQuery { get; set; }

        public JobStatus JobStatus { get; set; }
    }
}