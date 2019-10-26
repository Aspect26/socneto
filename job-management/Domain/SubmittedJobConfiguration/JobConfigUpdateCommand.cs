using System;
using System.Collections.Generic;

namespace Domain.SubmittedJobConfiguration
{
    public enum JobCommand
    {
        New,Stop
    }

    public enum JobStatus
    {
        Stopped, Running
    }

    public class JobConfigUpdateCommand
    {
        private JobConfigUpdateCommand(
            Guid jobId,
            List<string> dataAnalysers,
            List<string> dataAcquirers,
            string topicQuery
        )
        {
            JobId = jobId;
            DataAnalysers = dataAnalysers;
            DataAcquirers = dataAcquirers;
            TopicQuery = topicQuery;
            
        }
        

        public Guid JobId { get; }
        public List<string> DataAnalysers { get; }
        public List<string> DataAcquirers { get; }
        public string TopicQuery { get; }
        

        public static JobConfigUpdateCommand NewJob(Guid jobId,
            List<string> analysers,
            List<string> acquirers,
            string topicQuery)
        {
            return new JobConfigUpdateCommand(
                jobId,
                analysers,
                acquirers, 
                topicQuery);
        }
    }
}