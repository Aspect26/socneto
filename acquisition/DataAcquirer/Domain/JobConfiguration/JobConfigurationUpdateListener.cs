using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.JobManagement;
using Domain.JobManagement.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class JobConfigurationUpdateListener
    {
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly IJobManager _jobManager;
        private readonly IEventTracker<JobConfigurationUpdateListener> _logger;
        private readonly string _updateChannelName;


        public JobConfigurationUpdateListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IJobManager jobManager,
            IOptions<ComponentOptions> componentOptionsAccessor,
            IEventTracker<JobConfigurationUpdateListener> logger)
        {

            _messageBrokerConsumer = messageBrokerConsumer;
            _jobManager = jobManager;
            _logger = logger;

            _updateChannelName = componentOptionsAccessor.Value.UpdateChannelName;
        }

        public Task ListenAsync(CancellationToken token)
        {
            _logger.TrackInfo("StartNewJob", "Starting listening",
                new { channelName = _updateChannelName });
            while (true)

            {
                return _messageBrokerConsumer.ConsumeAsync(
                    _updateChannelName,
                    ProcessJobConfigAsync,
                    token);

            }
        }

        private async Task ProcessJobConfigAsync(string configJson)
        {
            DataAcquirerJobConfig jobConfig;
            try
            {
                jobConfig = JsonConvert.DeserializeObject<DataAcquirerJobConfig>(configJson);
            }
            catch (JsonException jre)
            {
                _logger.TrackError(
                       "StartNewJob",
                       "Could not parse job config",
                       new
                       {
                           config=configJson,
                           exception = jre
                       });
                throw new InvalidOperationException($"Could not parse job config {jre.Message}");
            }

            if (jobConfig.Command == null)
            {
                const string error = "Job notification was not processed. Empty command";
                _logger.TrackError(
                        "StartNewJob",
                        error,
                        new
                        {
                            jobId = jobConfig.JobId,
                        });
                throw new InvalidOperationException(error);
            }

            // TODO separate constants
            var stopIdentifier = "stop";
            var startIdentifier = "start";
            var command = jobConfig.Command.ToLower();
            if (command == stopIdentifier)
            {
                await _jobManager.StopJobAsync(jobConfig.JobId);
            }
            else if (command == startIdentifier)
            {
                try
                {
                    await _jobManager.StartNewJobAsync(jobConfig);
                }
                catch (JobException e)
                {
                    _logger.TrackError(
                        "StartNewJob", 
                        "Job failed to start", 
                        new { 
                            jobId = jobConfig.JobId ,
                            exception=e
                        });
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"Invalid command identifier {jobConfig.Command}");
            }
        }
    }
}
