using System;
using System.Collections.Generic;

namespace Domain.SubmittedJobConfiguration
{
    public class JobConfigUpdateNotification
    {
        public JobConfigUpdateNotification(
            Guid jobId,
            List<string> analysers,
            List<string> networks,
            string topicQuery
        )
        {
            JobId = jobId;
            Analysers = analysers;
            Networks = networks;
            TopicQuery = topicQuery;
        }

        public Guid JobId { get; }
        public List<string> Analysers { get; }
        public List<string> Networks { get; }
        public string TopicQuery { get; }
    }
}