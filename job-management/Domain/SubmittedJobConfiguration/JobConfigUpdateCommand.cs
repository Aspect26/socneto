﻿using Newtonsoft.Json.Linq;
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
            string language,
            JObject attributes)

        {
            JobId = jobId;
            JobName = jobName;
            DataAnalysers = dataAnalysers;
            DataAcquirers = dataAcquirers;
            TopicQuery = topicQuery;
            Attributes = attributes;
            Language = language;
        }


        public Guid JobId { get; }
        public string JobName { get; }
        public List<string> DataAnalysers { get; }
        public List<string> DataAcquirers { get; }
        public string TopicQuery { get; }
        public string Language { get; }
      
        public JObject Attributes { get; } 

        public static JobConfigUpdateCommand NewJob(Guid jobId,
            string jobName,
            List<string> analysers,
            List<string> acquirers,
            string topicQuery,
            string language,
            JObject attributes)
        {
            return new JobConfigUpdateCommand(
                jobId,
                jobName,
                analysers,
                acquirers,
                topicQuery,
                language,
                attributes);
        }
    }
}