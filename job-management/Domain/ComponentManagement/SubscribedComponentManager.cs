using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly IComponentRegistry _componentRegistry;
        private readonly IComponentConfigUpdateNotifier _componentConfigUpdateNotifier;
        private readonly ILogger<SubscribedComponentManager> _logger;

        public SubscribedComponentManager(
            IComponentRegistry componentRegistry,
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
            
            //var storage = _componentRegistry.GetRegisteredStorage();
            //if (storage == null)
            //{
            //    _logger.LogWarning("No storage component was registered");
            //    //throw new InvalidOperationException("No storage is present. Job can't be done");
            //}

            //var storageChannelName = storage.InputChannelName;

            //await PushStorageJobConfig(
            //    storage,
            //    jobConfigUpdateNotification);

            var aStorageChannelName = "job_management.component_data_analyzed_input.storage_db";
            var analysers = await PushAnalyserJobConfig(
                aStorageChannelName,
                jobConfigUpdateNotification);

            var analysersInputs = analysers.Select(r => r.InputChannelName).ToArray();
            var dStorageChannelName = "job_management.component_data_input.storage_db";
            await PushNetworkDataAcquisitionJobConfig(
                dStorageChannelName,
                analysersInputs,
                jobConfigUpdateNotification);
        }

        private async Task PushStorageJobConfig(
            SubscribedComponent storageComponent,
            JobConfigUpdateNotification jobConfigUpdateNotification)
        {
            var notification = new StorageConfigUpdateNotification
            {
                JobId = jobConfigUpdateNotification.JobId,
                Attributes = new Dictionary<string, string>()
                {

                }
            };

            await _componentConfigUpdateNotifier.NotifyComponentAsync(
                storageComponent.UpdateChannelName,
                notification);
            _logger.LogInformation("Config pushed to: {componentName}", storageComponent.ComponentId);
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

        private async Task PushNetworkDataAcquisitionJobConfig(
            string storageChannelName,
            IEnumerable<string> selectedAnalysersChannels,
            JobConfigUpdateNotification jobConfigUpdateNotification)
        {

            var outputChannels = selectedAnalysersChannels.Concat(new[] { storageChannelName, }).ToArray();

            var notification = new DataAcquisitionConfigUpdateNotification
            {
                JobId = jobConfigUpdateNotification.JobId,
                Attributes = new Dictionary<string, string>()
                {
                    {"TopicQuery", jobConfigUpdateNotification.TopicQuery }
                },
                OutputMessageBrokerChannels = outputChannels,
            };

            foreach (var network in jobConfigUpdateNotification.Networks)
            {
                if (_componentRegistry.TryGetNetworkComponent(network, out var networkCmp))
                {
                    _logger.LogInformation("Config pushed to: {componentName}, config: {config}",
                        network,
                        JsonConvert.SerializeObject(notification));
                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        networkCmp.UpdateChannelName,
                        notification);
                }
                else
                {
                    const string errorMessage =
                        "Network data acquisition component {analyserName} was not registered";
                    _logger.LogWarning(errorMessage, network);
                }
            }
        }

        private async Task<List<SubscribedComponent>> PushAnalyserJobConfig(
            string storageChannelName,
            JobConfigUpdateNotification jobConfigUpdateNotification)
        {

            var analysers = new List<SubscribedComponent>();
            foreach (var analyser in jobConfigUpdateNotification.Analysers)
            {
                if (_componentRegistry.TryGetAnalyserComponent(analyser, out var analyserCmp))
                {
                    analysers.Add(analyserCmp);
                }
                else
                {
                    _logger.LogWarning("Analyser {analyserName} was not registered", analyser);
                }
            }


            var notification = new AnalyserConfigUpdateNotification()
            {
                JobId = jobConfigUpdateNotification.JobId,
                Attributes = new Dictionary<string, string>(),
                OutputMessageBrokerChannels = new[] { storageChannelName },
            };
            var configUpdateTasks = analysers.Select(analyserCmp =>
            {
                _logger.LogInformation("Config pushed to: {componentName}, config: {config}",
                    analyserCmp,
                    JsonConvert.SerializeObject(notification));
                var analyserTask = _componentConfigUpdateNotifier.NotifyComponentAsync(
                    analyserCmp.UpdateChannelName,
                    notification);


                return analyserTask;

            });
            await Task.WhenAll(configUpdateTasks);
            return analysers;
        }
    }

}
