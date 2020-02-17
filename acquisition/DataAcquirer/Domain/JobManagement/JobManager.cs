using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.EventTracking;
using Domain.Exceptions;
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
        private readonly ITranslationService _translationService;
        private readonly IMessageBrokerProducer _producer;

        private readonly IEventTracker<JobManager> _logger;

        // concurent dictionary does not suffice
        private bool _isStopping = false;
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<Guid, JobManagerJobRecord> _runningJobsRecords
            = new Dictionary<Guid, JobManagerJobRecord>();


        public JobManager(
            IDataAcquirerJobStorage dataAcquirerJobStorage,
            IDataAcquirer acquirer,
            ITranslationService translationService,
            IMessageBrokerProducer producer,
            IEventTracker<JobManager> logger)
        {
            _dataAcquirerJobStorage = dataAcquirerJobStorage;
            //   _jobMetadataStorage = jobMetadataStorage;
            _acquirer = acquirer;
            _translationService = translationService;
            _producer = producer;
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
                                new { jobId = jobId.ToString() });
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

        private async Task RunJobAsync(DataAcquirerJobConfig jobConfig,
            CancellationToken cancellationToken)
        {
            try
            {
                var translate = jobConfig.Attributes.TryGetValue("Translate",out string value) 
                    && value.ToLower() == "true";
                // TODO validate job config
                if (!jobConfig.Attributes.ContainsKey("TopicQuery"))
                {
                    _logger.TrackError(
                        "StartNewJob",
                        "TopicQuery attribute is not present. Job did not start",
                        new { jobId = jobConfig.JobId });
                    return;
                }
                string queryLanguage = null;
                if (jobConfig.Attributes.TryGetValue("Language", out var desiredLanguage))
                {
                    queryLanguage = desiredLanguage;
                }

                await _dataAcquirerJobStorage.SaveAsync(jobConfig.JobId, jobConfig);

                var batchSize = 100;

                var dataAcquirerInputModel = DataAcquirerInputModel.FromValues(
                   jobConfig.JobId,
                   jobConfig.Attributes["TopicQuery"],
                   queryLanguage,
                   new DataAcquirerAttributes(jobConfig.Attributes),
                   batchSize
               );

                var batch = _acquirer.GetPostsAsync(
                    dataAcquirerInputModel,
                    cancellationToken);

                _logger.TrackInfo("MessageTracking", "Starting");

                await ProcessBatch(jobConfig, dataAcquirerInputModel, batch,translate);

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
                        jobId = jobConfig.JobId,
                        exception = e
                    });
            }
        }

        private async Task ProcessBatch(
            DataAcquirerJobConfig jobConfig, 
            DataAcquirerInputModel dataAcquirerInputModel, 
            IAsyncEnumerable<DataAcquirerPost> batch,
            bool translate)
        {
            int count = 0;
            await foreach (var dataPost in batch)
            {
                LogProgress(jobConfig, count);
                count++;

                var postId = CalculatePostId(jobConfig, dataPost);

                var text = ClearText(dataPost.Text);
                string originalText = null;

                if (translate && dataPost.Language != "en" && dataPost.Language != null)
                {
                    try
                    {
                        var translatedText = await _translationService
                                .TranslateToEnglishAsync(dataPost.Language, text);

                        originalText = text;
                        text = translatedText;
                    }
                    catch (DataAcquirerException ex)
                    {
                        _logger.TrackWarning("TranslationError", "Could not translate",
                            new
                            {
                                jobId = dataAcquirerInputModel.JobId,
                                exception = ex,
                                text
                            });
                    }
                }

                var uniPost = UniPostModel.FromValues(
                    postId,
                    dataPost.OriginalPostId,
                    text,
                    originalText,
                    dataPost.Language,
                    dataPost.Source,
                    dataPost.UserId,
                    dataPost.DateTime,
                    dataAcquirerInputModel.JobId,
                    dataPost.Query);


                var jsonData = JsonConvert.SerializeObject(uniPost);
                var messageBrokerMessage = new MessageBrokerMessage(
                    "acquired-data-post",
                    jsonData);

                await SendRecordToOutputs(jobConfig.OutputMessageBrokerChannels,
                    messageBrokerMessage);
            }
        }

        private void LogProgress(DataAcquirerJobConfig jobConfig, int count)
        {
            if (count % 1000 == 0)
            {
                _logger.TrackInfo("MessageTracking", $"Downloaded: {count}", new
                {
                    jobId = jobConfig.JobId
                });
            }
        }

        private static Guid CalculatePostId(DataAcquirerJobConfig jobConfig, DataAcquirerPost dataPost)
        {
            var bytes = new byte[16];

            var textHash = dataPost.Text.GetHashCode();
            var postIdHash = dataPost.OriginalPostId.GetHashCode();
            var userIdHash = dataPost.UserId.GetHashCode();
            var dateIdHash = dataPost.DateTime.GetHashCode();
            var jobId = jobConfig.JobId.GetHashCode();
            dateIdHash += jobId;

            BitConverter.GetBytes(textHash).CopyTo(bytes, 0);
            BitConverter.GetBytes(postIdHash).CopyTo(bytes, 3);
            BitConverter.GetBytes(userIdHash).CopyTo(bytes, 7);
            BitConverter.GetBytes(dateIdHash).CopyTo(bytes, 11);

            var postId = new Guid(bytes);
            return postId;
        }

        private string ClearText(string text)
        {
            return Regex.Replace(text, @"\s+", " ").Trim();
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
