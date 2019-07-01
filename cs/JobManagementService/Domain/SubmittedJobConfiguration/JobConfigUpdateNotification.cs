using System.Collections.Generic;

namespace Domain.SubmittedJobConfiguration
{
    public class JobConfigUpdateNotification
    {
        public JobConfigUpdateNotification(
            List<string> analysers,
            List<string> networks,
            string topicQuery
        )
        {
            Analysers = analysers;
            Networks = networks;
            TopicQuery = topicQuery;
        }
        public List<string> Analysers { get; }
        public List<string> Networks { get; }
        public string TopicQuery { get; }
    }
}