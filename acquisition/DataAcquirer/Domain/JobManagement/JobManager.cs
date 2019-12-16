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
using Domain.JobManagement.Abstract;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.JobManagement
{

    public class JobManager : IJobManager, IDisposable
    {
        private readonly IDataAcquirerJobStorage _dataAcquirerJobStorage;
        private readonly IDataAcquirer _acquirer;
        private readonly IMessageBrokerProducer _producer;
        private readonly IDataAcquirerMetadataContextProvider _dataAcquirerMetadataContextProvider;
        private readonly IEventTracker<JobManager> _logger;

        // concurent dictionary does not suffice
        private bool _isStopping = false;
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<Guid, JobManagerJobRecord> _runningJobsRecords
            = new Dictionary<Guid, JobManagerJobRecord>();


        public JobManager(
            IDataAcquirerJobStorage dataAcquirerJobStorage,
            IDataAcquirer acquirer,
            IMessageBrokerProducer producer,
            IDataAcquirerMetadataContextProvider dataAcquirerMetadataContextProvider,
            IEventTracker<JobManager> logger)
        {
            _dataAcquirerJobStorage = dataAcquirerJobStorage;
            //   _jobMetadataStorage = jobMetadataStorage;
            _acquirer = acquirer;
            _producer = producer;
            _dataAcquirerMetadataContextProvider = dataAcquirerMetadataContextProvider;
            _logger = logger;
        }

        public Task StartNewJobAsync(DataAcquirerJobConfig jobConfig)
        {
            lock (_dictionaryLock)
            {
                var jobId = jobConfig.JobId;
                if (_isStopping)
                {
                    _logger.TrackWarning(
                        "StartNewJob",
                        "Could not start job, because the component is stopping",
                        new { jobId = jobId });

                    return Task.CompletedTask;
                }

                if (_runningJobsRecords.ContainsKey(jobId))
                {
                    _logger.TrackWarning(
                        "StartNewJob",
                        "Job is with this id already running",
                        new { jobId = jobId });
                    return Task.CompletedTask;
                }

                _logger.TrackInfo(
                    "StartNewJob",
                    "Config recieved",
                    new { config = jobConfig });

                var cancellationTokenSource = new CancellationTokenSource();
                var downloadingTask = RunJobAsync(jobConfig, cancellationTokenSource.Token).
                    ContinueWith(async r =>
                        {
                            try
                            {
                                await r;
                            }
                            catch (TaskCanceledException) { }

                            _runningJobsRecords.Remove(jobId, out _);
                            _logger.TrackInfo(
                                "StartNewJob",
                                "Job removed",
                                new { config = jobId });
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

        private async Task RunJobAsync(DataAcquirerJobConfig jobConfig, CancellationToken cancellationToken)
        {
            try
            {
                // TODO validate job config
                if (!jobConfig.Attributes.ContainsKey("TopicQuery"))
                {
                    _logger.TrackError(
                        "StartNewJob",
                        "TopicQuery attribute is not present. Job did not start",
                        new { jobId = jobConfig.JobId });
                    return;
                }
                var queryLanguage = "en";
                var supported = new[] { "en", "cs" };
                if (jobConfig.Attributes.TryGetValue("Language", out var desiredLanguage))
                {
                    if (supported.Contains(desiredLanguage))
                    {
                        queryLanguage = desiredLanguage;
                    }
                    else
                    {
                        _logger.TrackError(
                            "StartNewJob",
                            "Unrecognized language",
                            new
                            {
                                language = desiredLanguage,
                                jobId = jobConfig.JobId
                            });
                    }
                }

                await _dataAcquirerJobStorage.SaveAsync(jobConfig.JobId, jobConfig);

                var earliestIdPagingParameter = ulong.MaxValue;
                ulong latestIdPagingParameter = 0;

                var batchSize = 100;

                var dataAcquirerInputModel = DataAcquirerInputModel.FromValues(
                   jobConfig.JobId,
                   jobConfig.Attributes["TopicQuery"],
                   queryLanguage,
                   new DataAcquirerAttributes(jobConfig.Attributes),
                   latestIdPagingParameter,
                   earliestIdPagingParameter,
                   batchSize
               );

                var context = _dataAcquirerMetadataContextProvider.Get(jobConfig.JobId);
                var batch = _acquirer.GetPostsAsync(context, dataAcquirerInputModel);

                await foreach (var dataPost in batch)
                {
                    var uniPost = UniPostModel.FromValues(
                        dataPost.PostId,
                        dataPost.Text,
                        dataPost.Source,
                        dataPost.UserId,
                        dataPost.PostDateTime,
                        dataAcquirerInputModel.JobId);

                    var jsonData = JsonConvert.SerializeObject(uniPost);
                    var messageBrokerMessage = new MessageBrokerMessage(
                        "acquired-data-post",
                        jsonData);

                    await SendRecordToOutputs(jobConfig.OutputMessageBrokerChannels,
                        messageBrokerMessage);
                }

            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                _runningJobsRecords.Remove(jobConfig.JobId);
                _logger.TrackError(
                    "RunJob",
                    "Job encountered an error and stopped.", 
                    new
                    {
                        jobId=jobConfig.JobId,
                        exception =e
                    });
            }
        }

        private async Task SendRecordToOutputs(string[] outputChannels,
            MessageBrokerMessage messageBrokerMessage)
        {
                _logger.TrackStatistics(
                    "SendingData",
                    new { channels = outputChannels });

            foreach (var outputChannel in outputChannels)
            {

                await _producer.ProduceAsync(outputChannel,
                    messageBrokerMessage);
            }
        }

        public async Task StopJobAsync(Guid jobId)
        {
            if (!_runningJobsRecords.TryGetValue(jobId, out var jobRecord))
            {
                _logger.TrackError("StopJob", "Job does not exist", new { jobId });
                var error = "Could not stop non existing job: {jobId}";
                throw new InvalidOperationException(string.Format(error, jobId));
            }

            jobRecord.CancellationTokenSource.Cancel();
            try
            {
                await jobRecord.JobTask;
            }
            catch (TaskCanceledException)
            {
                // intentionally emptyy
            }

            _runningJobsRecords.Remove(jobId);
            await _dataAcquirerJobStorage.RemoveJobAsync(jobId);
        }

        public async Task StopAllJobsAsync()
        {
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


        public void Dispose()
        {
            // TODO dispose the jobs
        }
    }




}
