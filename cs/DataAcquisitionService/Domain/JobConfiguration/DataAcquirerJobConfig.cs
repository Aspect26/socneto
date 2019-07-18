using System;

namespace Domain.JobConfiguration
{
    public class DataAcquirerJobConfig
    {
        public Guid JobId { get; set; }
        public string Query { get; set; }
        public string OutputChannelName { get; set; }
    }
}