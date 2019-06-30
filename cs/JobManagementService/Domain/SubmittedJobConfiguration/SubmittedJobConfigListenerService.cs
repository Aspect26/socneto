using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.SubmittedJobConfiguration
{
    public class SubmittedJobConfigListenerService
    {
        private readonly ISubscribedComponentManager _subscribedComponentManager;
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly ILogger<SubmittedJobConfigListenerService> _logger;
        private readonly string  _jobConfigChannelName;

        public SubmittedJobConfigListenerService(
            ISubscribedComponentManager subscribedComponentManager,
            IMessageBrokerConsumer messageBrokerConsumer,
            IOptions<SubmittedJobConfigListenerOptions> submittedJobConfigListenerOptionsAccessor, 
            ILogger<SubmittedJobConfigListenerService> logger)
        {
            _subscribedComponentManager = subscribedComponentManager;
            _messageBrokerConsumer = messageBrokerConsumer;

            if (string.IsNullOrWhiteSpace(submittedJobConfigListenerOptionsAccessor.Value.JobConfigChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name",
                    nameof(submittedJobConfigListenerOptionsAccessor.Value.JobConfigChannelName));
            }

            _jobConfigChannelName = submittedJobConfigListenerOptionsAccessor.Value.JobConfigChannelName;
            
            _logger = logger;
        }
        public Task Listen(CancellationToken cancellationToken)
        {
            return _messageBrokerConsumer.ConsumeAsync(
                _jobConfigChannelName,
                ProcessJobConfigUpdate,
                cancellationToken);
        }

        private async Task ProcessJobConfigUpdate(string jobConfigUpdate)
        {
            _logger.LogInformation("Job config accepted: {registrationRequestJson}",
                jobConfigUpdate);

            var jobConfigUpdateNotification 
                = JsonConvert.DeserializeObject<JobConfigUpdateNotification>(jobConfigUpdate);
            await _subscribedComponentManager.PushJobConfigUpdateAsync(jobConfigUpdateNotification);
        }
    }
}