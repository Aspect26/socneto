using System;

namespace Domain.JobConfiguration
{
    public class DataAnalyzerJobConfig
    {
        public Guid JobId { get; set; }
        public string OutputChannelName { get; set; }
    }
}