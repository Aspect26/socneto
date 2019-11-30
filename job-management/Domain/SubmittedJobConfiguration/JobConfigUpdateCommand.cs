using System;
using System.Collections.Generic;

namespace Domain.SubmittedJobConfiguration
{
    public enum JobCommand
    {
        Start, Stop
    }

    public enum JobStatus
    {
        Stopped, Running
    }

    public class JobConfigUpdateCommand
    {
        private JobConfigUpdateCommand(
            Guid jobId,
            string jobName,
            List<string> dataAnalysers,
            List<string> dataAcquirers,
            string topicQuery,
            Dictionary<string, Dictionary<string, string>> attributes)
        {
            JobId = jobId;
            JobName = jobName;
            DataAnalysers = dataAnalysers;
            DataAcquirers = dataAcquirers;
            TopicQuery = topicQuery;
            Attributes = attributes;
        }


        public Guid JobId { get; }
        public string JobName { get; }
        public List<string> DataAnalysers { get; }
        public List<string> DataAcquirers { get; }
        public string TopicQuery { get; }

        public Dictionary<string, Dictionary<string, string>> Attributes { get; }

        public static JobConfigUpdateCommand NewJob(Guid jobId,
            string jobName,
            List<string> analysers,
            List<string> acquirers,
            string topicQuery,
            Dictionary<string, Dictionary<string, string>> attributes)
        {
            return new JobConfigUpdateCommand(
                jobId,
                jobName,
                analysers,
                acquirers,
                topicQuery,
                attributes);
        }
    }
}