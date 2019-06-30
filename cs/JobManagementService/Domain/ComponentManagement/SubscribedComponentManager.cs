using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly IComponentRegistry _componentRegistry;
        private readonly IComponentConfigUpdateNotifier _componentConfigUpdateNotifier;
        private readonly ILogger<SubscribedComponentManager> _logger;

        public SubscribedComponentManager(IComponentRegistry componentRegistry,
            IComponentConfigUpdateNotifier componentConfigUpdateNotifier,
            ILogger<SubscribedComponentManager> logger
        )
        {
            _componentRegistry = componentRegistry;
            _componentConfigUpdateNotifier = componentConfigUpdateNotifier;
            _logger = logger;
        }

        public void SubscribeComponent(ComponentRegistrationModel componentRegistrationModel)
        {
            var registered = _componentRegistry.AddOrUpdate(componentRegistrationModel);
            if (!registered)
            {
                throw new InvalidOperationException(
                    $"Device {componentRegistrationModel.ComponentId} already exists");
            }
        }

        public async Task PushJobConfigUpdateAsync(JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            await PushAnalyserJobConfig(jobConfigUpdateNotification);

            await PushNetworkDataAcquisitionJobConfig(jobConfigUpdateNotification);
        }

        private async Task PushNetworkDataAcquisitionJobConfig(JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            foreach (var network in jobConfigUpdateNotification.Networks)
            {
                if (_componentRegistry.TryGetNetworkComponent(network, out var networkCmp))
                {
                    var notification = new ComponentConfigUpdateNotification
                    {
                        Attributes = new Dictionary<string, string>()
                        {
                            {"TopicQuery", jobConfigUpdateNotification.TopicQuery}
                        }
                    };

                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        networkCmp.ChannelName,
                        notification);
                }
                else
                {
                    const string errorMessage = "Network data acquisition component {analyserName} was not registered";
                    _logger.LogWarning(errorMessage, network);
                }
            }
        }

        private async Task PushAnalyserJobConfig(JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            foreach (var analyser in jobConfigUpdateNotification.Analysers)
            {
                if (_componentRegistry.TryGetAnalyserComponent(analyser, out var analyserCmp))
                {
                    var notification = new ComponentConfigUpdateNotification
                    {
                        // TODO
                    };

                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        analyserCmp.ChannelName,
                        notification);
                }
                else
                {
                    _logger.LogWarning("Analyser {analyserName} was not registered", analyser);
                }
            }
        }
    }
}
