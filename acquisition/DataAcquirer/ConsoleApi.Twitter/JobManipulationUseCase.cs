using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Domain.Registration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.Twitter
{
    public class JobManipulationUseCase
    {
        private readonly TwitterCredentialsOptions _twitterCredentials;
        private readonly IRegistrationService _registrationService;
        private readonly JobConfigurationUpdateListenerHostedService _jobConfigurationUpdateListenerHostedService;
        private readonly InteractiveConsumer _interactiveConsumer;
        private readonly ILogger<JobManipulationUseCase> _logger;
        private readonly ComponentOptions _componentOptions;

        public JobManipulationUseCase(
            IRegistrationService registrationService,
            IMessageBrokerConsumer interactiveConsumer,
            IOptions<TwitterCredentialsOptions> twitterCredentialsOptionsAccessor,
            JobConfigurationUpdateListenerHostedService jobConfigurationUpdateListenerHostedService,
            IOptions<ComponentOptions> componentOptionsAccessor,
            ILogger<JobManipulationUseCase> logger)
        {
            _twitterCredentials = twitterCredentialsOptionsAccessor.Value;
            _registrationService = registrationService;
            _jobConfigurationUpdateListenerHostedService = jobConfigurationUpdateListenerHostedService;
            _interactiveConsumer = interactiveConsumer as InteractiveConsumer;
            _logger = logger;
            _componentOptions = componentOptionsAccessor.Value;
        }


        public async Task SimpleStartStop()
        {
            await _jobConfigurationUpdateListenerHostedService.StartAsync(CancellationToken.None);

            await Register();

            var jobId = Guid.NewGuid();

            await StartJob(jobId);
            await Task.Delay(TimeSpan.FromMinutes(1));
            await StopJob(jobId);
            await _jobConfigurationUpdateListenerHostedService.StopAsync(CancellationToken.None);

        }

        private async Task StartJob(Guid jobId)
        {
            var attributes = new Dictionary<string, string>() {
                {"TopicQuery","matfyz" },
                { "ApiKey" , _twitterCredentials.ApiKey},
                {"ApiSecretKey" , _twitterCredentials.ApiSecretKey},
                {"AccessToken" ,_twitterCredentials.AccessToken},
                {"AccessTokenSecret" , _twitterCredentials.AccessTokenSecret}
            };
            
            var dataAcquirerJobConfig = new DataAcquirerJobConfig()
            {
                Attributes = attributes,
                Command = "start",
                OutputMessageBrokerChannels = new[] { "c1" },
                JobId = jobId,
            };

            await _interactiveConsumer.AddMessageToBeConsumed(
                _componentOptions.UpdateChannelName,
                dataAcquirerJobConfig);
        }

        private async Task StopJob(Guid jobId)
        {
            var dataAcquirerJobConfig = new DataAcquirerJobConfig()
            {
                Command = "stop",
                JobId = jobId,
            };

            await _interactiveConsumer.AddMessageToBeConsumed(
                _componentOptions.UpdateChannelName,
                dataAcquirerJobConfig);
        }

        private async Task Register()
        {
            var registrationRequest = new RegistrationRequest()
            {
                ComponentId = _componentOptions.ComponentId,
                ComponentType = _componentOptions.ComponentType,
                InputChannelName = _componentOptions.InputChannelName,
                UpdateChannelName = _componentOptions.UpdateChannelName
            };

            while (true)
            {
                try
                {
                    _logger.LogInformation("Sending registration request");
                    await _registrationService.Register(registrationRequest);
                    _logger.LogInformation(
                        "Service {serviceName} register request sent",
                        "DataAcquisitionService");
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError("Registration failed. Error: {error}", e.Message);
                    _logger.LogInformation("trying again in 30 seconds");
                    await Task.Delay(TimeSpan.FromMinutes(.5));
                }
            }
        }
    }

    public class InteractiveConsumer : IMessageBrokerConsumer
    {
        private readonly ILogger<InteractiveConsumer> _logger;
        ConcurrentDictionary<string, Queue<string>> _channels
            = new ConcurrentDictionary<string, Queue<string>>();

        public InteractiveConsumer(
            ILogger<InteractiveConsumer> logger)
        {
            _logger = logger;
        }
        public Task AddMessageToBeConsumed(string channel, object messageObject)
        {
            var message = JsonConvert.SerializeObject(messageObject);

            _channels.AddOrUpdate(channel,
                (channelName) => new Queue<string>(new[] { message }),
                (channelName, queue) => { queue.Enqueue(message); return queue; });

            return Task.CompletedTask;
        }

        public async Task ConsumeAsync(
            string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                if (!_channels.TryGetValue(consumeTopic, out var queue) || queue.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    continue;
                }

                var message = queue.Dequeue();
                try
                {
                    await onRecieveAction(message);
                }
                catch (Exception e)
                {
                    _logger.LogError("Consuming error: {error}", e.Message);
                }
            }
        }
    }

}
