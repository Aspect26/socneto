using System;

namespace Domain.PostAnalysis
{
    public class DataAcquirerJobConfig
    {
        public Guid JobId { get; set; }
        public string Query { get; set; }
        public string OutputChannelName { get; set; }
    }
}