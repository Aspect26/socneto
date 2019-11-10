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
        private readonly ILogger<JobConfigurationUpdateListener> _logger;
        private readonly string _updateChannelName;


        public JobConfigurationUpdateListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IJobManager jobManager,
            IOptions<ComponentOptions> componentOptionsAccessor,
            ILogger<JobConfigurationUpdateListener> logger)
        {

            _messageBrokerConsumer = messageBrokerConsumer;
            _jobManager = jobManager;
            _logger = logger;

            _updateChannelName = componentOptionsAccessor.Value.UpdateChannelName;
        }

        public Task ListenAsync(CancellationToken token)
        {
            _logger.LogInformation("Starting listening at {topic}", _updateChannelName);
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
            catch (JsonReaderException jre)
            {
                _logger.LogError("Could not parse job config: Error: {error}, config {jobConfigJson}",
                    jre.Message,
                    configJson);
                throw new InvalidOperationException($"Could not parse job config {jre.Message}");
            }

            if (jobConfig.Command == null)
            {
                const string error = "Job notification was not processed. Empty command";
                _logger.LogError(error);
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
                await _jobManager.StartNewJobAsync(jobConfig);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"Invalid command identifier {jobConfig.Command}");
            }
        }
    }
}
