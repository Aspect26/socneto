using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.JobManagement;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class JobConfigurationUpdateListener
    {
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly IJobManager _jobManager;
        private readonly string _updateChannelName;
        public bool ConnectionEstablished { get; private set; } = false;


        public JobConfigurationUpdateListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IJobManager jobManager,
            IOptions<ComponentOptions> componentOptionsAccessor)
        {
            if (string.IsNullOrEmpty(componentOptionsAccessor.Value.UpdateChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name",
                    nameof(componentOptionsAccessor.Value.UpdateChannelName));
            }

            _messageBrokerConsumer = messageBrokerConsumer;
            _jobManager = jobManager;

            _updateChannelName = componentOptionsAccessor.Value.UpdateChannelName;
        }

        public void OnConnectionEstablished()
        {
            ConnectionEstablished = true;
        }
        public Task ListenAsync(CancellationToken token)
        {
            return _messageBrokerConsumer.ConsumeAsync(
                _updateChannelName,
                ProcessJobConfigAsync,
                token);
        }

        private async Task ProcessJobConfigAsync(string configJson)
        {
            var jobConfig = JsonConvert.DeserializeObject<DataAcquirerJobConfig>(configJson);
            await _jobManager.StartDownloadingAsync(jobConfig);
        }
    }
}
