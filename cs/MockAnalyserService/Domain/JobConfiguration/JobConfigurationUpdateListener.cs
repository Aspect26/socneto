using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class JobConfigurationUpdateListener
    {
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly IJobManager _jobManager;

        private readonly string _updateChannelName;


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
        
        public Task ListenAsync(CancellationToken token)
        {
            return _messageBrokerConsumer.ConsumeAsync(
                _updateChannelName,
                ProcessJobConfigAsync,
                token);
        }

        private Task ProcessJobConfigAsync(string configJson)
        {
            var jobConfig = JsonConvert.DeserializeObject<DataAnalyzerJobConfig>(configJson);
            _jobManager.Register(jobConfig);
            return Task.CompletedTask;
        }
    }
}