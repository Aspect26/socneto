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
        public List<string> Analysers { get; set; }
        public List<string> Networks { get; set; }
        public string TopicQuery { get; set; }
    }
}