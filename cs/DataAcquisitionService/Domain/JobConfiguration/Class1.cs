using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class JobConfigurationUpdateListenerHostedService : IHostedService
    {
        private readonly JobConfigurationUpdateListener _jobConfigurationUpdateListener;
        private readonly ILogger<JobConfigurationUpdateListenerHostedService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _listenTask;

        public JobConfigurationUpdateListenerHostedService(
            JobConfigurationUpdateListener jobConfigurationUpdateListener,
            ILogger<JobConfigurationUpdateListenerHostedService> logger)
        {
            _jobConfigurationUpdateListener = jobConfigurationUpdateListener;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} started",
                nameof(JobConfigurationUpdateListenerHostedService));

            // wait until this module gets registered
            while (!_jobConfigurationUpdateListener.ConnectionEstablished)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            
            _listenTask = _jobConfigurationUpdateListener.ListenAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} is stopping",
                nameof(JobConfigurationUpdateListenerHostedService));

            _cancellationTokenSource.Cancel();
            await _listenTask;

            _logger.LogInformation("Service {serviceName} stopped",
                nameof(JobConfigurationUpdateListenerHostedService));
        }
    }

    public class JobConfigurationUpdateListener
    {
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly IJobConfigUpdateProcessor _jobConfigUpdateProcessor;
        private readonly string _updateChannelName;
        public bool ConnectionEstablished { get; private set; } = false;


        public JobConfigurationUpdateListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IJobConfigUpdateProcessor jobConfigUpdateProcessor,
            IOptions<ComponentOptions> componentOptionsAccessor)
        {
            if (string.IsNullOrEmpty(componentOptionsAccessor.Value.UpdateChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name",
                    nameof(componentOptionsAccessor.Value.UpdateChannelName));
            }

            _messageBrokerConsumer = messageBrokerConsumer;
            _jobConfigUpdateProcessor = jobConfigUpdateProcessor;

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
            await _jobConfigUpdateProcessor.ProcessConfigAsync(jobConfig);
        }
    }

    public class DataAcquirerJobConfig
    {

    }


    public interface IJobConfigUpdateProcessor
    {
        Task ProcessConfigAsync(DataAcquirerJobConfig jobConfig);
    }

    public class JobConfigUpdateProcessor : IJobConfigUpdateProcessor
    {
        private readonly ILogger<JobConfigUpdateProcessor> _logger;

        public JobConfigUpdateProcessor(ILogger<JobConfigUpdateProcessor> logger)
        {
            _logger = logger;
        }
        public Task ProcessConfigAsync(DataAcquirerJobConfig jobConfig)
        {
            var json = JsonConvert.SerializeObject(jobConfig);
            _logger.LogInformation("Config recieved {}",json);
            return Task.CompletedTask;
        }
    }
}
