using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Analyser;
using Domain.JobConfiguration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain
{
    public interface IJobManager
    {
        void Register(DataAnalyzerJobConfig jobConfig);
        string[] GetJobConfigOutput(Guid postJobId);
    }
    
    public class JobManager : IJobManager
    {
        private class JobManagerJobRecord
        {
            public Guid JobId { get; set; }
            public string[] Outputs { get; set; }
        }
        
        private readonly ILogger<JobManager> _logger;

        // concurent dictionary does not suffice
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<Guid, JobManagerJobRecord> _runningJobsRecords
            = new Dictionary<Guid, JobManagerJobRecord>();


        public JobManager(
            ILogger<JobManager> logger)
        {
            _logger = logger;
        }
        


        public void Register(DataAnalyzerJobConfig jobConfig)
        {
            lock (_dictionaryLock)
            {
                var json = JsonConvert.SerializeObject(jobConfig);
                _logger.LogInformation("Config received {}", json);

                var jobManagerJobRecord = new JobManagerJobRecord
                {
                    JobId = jobConfig.JobId,
                    Outputs = jobConfig.OutputChannelNames
                };

                _runningJobsRecords.TryAdd(jobManagerJobRecord.JobId, jobManagerJobRecord);
                
            }
        }

        public string[] GetJobConfigOutput(Guid postJobId)
        {
            return _runningJobsRecords[postJobId].Outputs;
        }
    }
}