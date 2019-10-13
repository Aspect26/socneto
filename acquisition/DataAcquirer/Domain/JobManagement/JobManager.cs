using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.JobManagement
{
    public interface IJobManager
    {
        Task StartDownloadingAsync(DataAcquirerJobConfig jobConfig);
    }
    public class JobManager : IJobManager
    {
        private class JobManagerJobRecord
        {
            public Guid JobId { get; set; }
            public Task JobTask { get; set; }
            public CancellationTokenSource CancellationTokenSource { get; set; }
        }

        private readonly IDataAcquirer _acquirer;
        private readonly IMessageBrokerProducer _producer;
        private readonly ILogger<JobManager> _logger;

        // concurent dictionary does not suffice
        private bool _isStopping = false;
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<Guid, JobManagerJobRecord> _runningJobsRecords
            = new Dictionary<Guid, JobManagerJobRecord>();


        public JobManager(
            IDataAcquirer acquirer,
            IMessageBrokerProducer producer,
            ILogger<JobManager> logger)
        {
            _acquirer = acquirer;
            _producer = producer;
            _logger = logger;
        }


        public Task StartDownloadingAsync(DataAcquirerJobConfig jobConfig)
        {
            lock (_dictionaryLock)
            {
                var jobId = jobConfig.JobId;
                if (_isStopping)
                {
                    _logger.LogWarning("Could not start downloading data of a job id {jobId}, because the component is stopping", jobId);
                    return Task.CompletedTask;
                }

                var json = JsonConvert.SerializeObject(jobConfig);
                _logger.LogInformation("Config recieved {config}", json);

                var cancellationTokenSource = new CancellationTokenSource();
                var downloadingTask = StartJobAsync(jobConfig, cancellationTokenSource.Token).
                    ContinueWith(async r =>
                        {
                            try
                            {
                                await r;
                            }
                            catch (TaskCanceledException) { }

                            _runningJobsRecords.Remove(jobId, out _);
                            _logger.LogInformation("Job {jobId} removed", jobId);

                        });

                var jobManagerJobRecord = new JobManagerJobRecord
                {
                    JobId = jobConfig.JobId,
                    JobTask = downloadingTask,
                    CancellationTokenSource = cancellationTokenSource
                };

                _runningJobsRecords.TryAdd(jobManagerJobRecord.JobId, jobManagerJobRecord);
                return Task.CompletedTask;
            }
        }

        private async Task StartJobAsync(DataAcquirerJobConfig jobConfig, CancellationToken cancellationToken)
        {
            try
            {

                ulong fromIdPagingParameter = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var dataAcquirerInputModel = new DataAcquirerInputModel
                    {
                        Query = jobConfig.Attributes["TopicQuery"],
                        JobId = jobConfig.JobId,
                        NumberOfPostToRetrieve = 100,
                        FromId = fromIdPagingParameter,
                        Attributes = new DataAcquirerAttributes(jobConfig.Attributes)
                    };

                    var data = await _acquirer.AcquireBatchAsync(
                        dataAcquirerInputModel,
                        cancellationToken);
                    fromIdPagingParameter = data.MaxId;
                    
                    foreach (var dataPost in data.Posts)
                    {
                        var jsonData = JsonConvert.SerializeObject(dataPost);
                        var messageBrokerMessage = new MessageBrokerMessage(
                            "acquired-data-post",
                            jsonData);

                        await SendToOutputs(jobConfig.OutputMessageBrokerChannels,
                            messageBrokerMessage);
                    }
                }
            }
            catch (TaskCanceledException) { }
        }

        private async Task SendToOutputs(string[] outputChannels,
            MessageBrokerMessage messageBrokerMessage)
        {
            foreach (var outputChannel in outputChannels)
            {
                await _producer.ProduceAsync(outputChannel,
                    messageBrokerMessage);
            }
        }

        public async Task StopDownloadingTasks()
        {
            // TODO handle concurency
            lock (_dictionaryLock)
            {
                _isStopping = true;
                foreach (var jobManagerJobRecord in _runningJobsRecords.Values)
                {
                    jobManagerJobRecord.CancellationTokenSource.Cancel();
                }
            }

            foreach (var jobRecord in _runningJobsRecords.Values)
            {
                try
                {
                    await jobRecord.JobTask;
                }
                catch (TaskCanceledException) { }
            }

            _runningJobsRecords.Clear();
        }


    }


}
