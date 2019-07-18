using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly IComponentRegistry _componentRegistry;
        private readonly IComponentConfigUpdateNotifier _componentConfigUpdateNotifier;
        private readonly ILogger<SubscribedComponentManager> _logger;
        private readonly string _analyserInputChannelName;
        private readonly string _analyserOutputChannelName;
        private readonly string _dataAcquisitionOutputChannelName;
        //private string _storageInputChannelName;

        public SubscribedComponentManager(
            IComponentRegistry componentRegistry,
            IComponentConfigUpdateNotifier componentConfigUpdateNotifier,
            IOptions<SubscribedComponentManagerOptions> subscribedComponentManagerOptionsAccessor, 
            ILogger<SubscribedComponentManager> logger
        )
        {
            _componentRegistry = componentRegistry;
            _componentConfigUpdateNotifier = componentConfigUpdateNotifier;
            if (string.IsNullOrWhiteSpace(
                subscribedComponentManagerOptionsAccessor.Value.AnalyserInputChannelName))
            {
                throw new ArgumentNullException();
            }
            _analyserInputChannelName = subscribedComponentManagerOptionsAccessor.Value.AnalyserInputChannelName;

            if (string.IsNullOrWhiteSpace(
                subscribedComponentManagerOptionsAccessor.Value.AnalyserOutputChannelName))
            {
                throw new ArgumentNullException();
            }

            _analyserOutputChannelName = subscribedComponentManagerOptionsAccessor.Value.AnalyserOutputChannelName;

            if (string.IsNullOrWhiteSpace(
                subscribedComponentManagerOptionsAccessor.Value.DataAcquisitionOutputChannelName))
            {
                throw new ArgumentNullException();
            }

            _dataAcquisitionOutputChannelName =
                subscribedComponentManagerOptionsAccessor.Value.DataAcquisitionOutputChannelName;
            //if (string.IsNullOrWhiteSpace(
            //    subscribedComponentManagerOptionsAccessor.Value.StorageInputChannelName))
            //{
            //    throw new ArgumentNullException();
            //}
            //_storageInputChannelName = subscribedComponentManagerOptionsAccessor.Value.StorageInputChannelName;
            
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
            await PushNetworkDataAcquisitionJobConfig(jobConfigUpdateNotification);
            await PushAnalyserJobConfig(jobConfigUpdateNotification);

        }

        public IList<SubscribedComponent> GetAvaliableNetworks()
        {
            return _componentRegistry.GetRegisteredComponents()
                .Where(r => r.ComponentType == "Network")
                .ToList();
        }

        public IList<SubscribedComponent> GetAvaliableAnalysers()
        {
            return _componentRegistry.GetRegisteredComponents()
                .Where(r => r.ComponentType == "Analyser")
                .ToList();
        }

        private async Task PushNetworkDataAcquisitionJobConfig(JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            foreach (var network in jobConfigUpdateNotification.Networks)
            {
                if (_componentRegistry.TryGetNetworkComponent(network, out var networkCmp))
                {
                    var notification = new DataAcquisitionConfigUpdateNotification
                    {
                        // TODO
                        //"TopicQuery", jobConfigUpdateNotification.TopicQuery,
                        //Attributes = new Dictionary<string, string>()
                        //{
                        //},
                        //OutputMessageBrokerChannel = _dataAcquisitionOutputChannelName,
                    };

                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        networkCmp.ChannelName,
                        notification);
                    _logger.LogInformation("Config pushed to: {componentName}", network);
                }
                else
                {
                    const string errorMessage = "Network data acquisition component {analyserName} was not registered";
                    _logger.LogWarning(errorMessage, network);
                }
            }
        }

        private async Task PushAnalyserJobConfig(
            JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            foreach (var analyser in jobConfigUpdateNotification.Analysers)
            {
                if (_componentRegistry.TryGetAnalyserComponent(analyser, out var analyserCmp))
                {
                    var notification = new AnalyserConfigUpdateNotification()
                    {
                        Attributes = new Dictionary<string, string>(),
                        OutputMessageBrokerChannel = _analyserOutputChannelName,
                        InputMessageBrokerChannel = _analyserInputChannelName
                    };

                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        analyserCmp.ChannelName,
                        notification);

                    _logger.LogInformation("Config pushed to: {componentName}", analyser);
                }
                else
                {
                    _logger.LogWarning("Analyser {analyserName} was not registered", analyser);
                }
            }
        }
    }
}
