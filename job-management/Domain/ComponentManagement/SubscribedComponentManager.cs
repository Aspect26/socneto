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

        public async Task<SubscribedComponentResultModel> SubscribeComponentAsync(
            ComponentRegistrationModel componentRegistrationModel)
        {
            try
            {
                var registered = await _componentRegistry
                    .AddOrUpdateAsync(componentRegistrationModel);

                if (!registered)
                {
                    return SubscribedComponentResultModel.AlreadyExists();
                }

                return SubscribedComponentResultModel.Successful();
            }
            catch (Exception e)
            {
                const string error = "Subscription failed due to: {error}";
                _logger.LogError(error, e.Message);
                return SubscribedComponentResultModel.Failed(string.Format(error, e.Message));
            }
        }

        public async Task PushJobConfigUpdateAsync(
            JobConfigUpdateNotification jobConfigUpdateNotification)
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
#warning hardcoded channel names

            var storeAnalysedDataChannelName = "job_management.component_data_analyzed_input.storage_db";
            var analysers = await PushAnalyserJobConfig(
                storeAnalysedDataChannelName,
                jobConfigUpdateNotification);

            var analysersInputs = analysers.Select(r => r.InputChannelName).ToArray();
            var storeRawDataChannelName = "job_management.component_data_input.storage_db";
            await PushNetworkDataAcquisitionJobConfig(
                storeRawDataChannelName,
                analysersInputs,
                jobConfigUpdateNotification);

            // TODO store the config in db
            var storeConfigChannelName = "job_management.job_definition.storage_db";




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

        private async Task PushNetworkDataAcquisitionJobConfig(
            string storageChannelName,
            IEnumerable<string> selectedAnalysersChannels,
            JobConfigUpdateNotification jobConfigUpdateNotification)
        {

            var outputChannels = selectedAnalysersChannels
                .Concat(new[] { storageChannelName, })
                .ToArray();

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
                var dataSource = await _componentRegistry.GetComponentById(network);

                if (dataSource == null)
                {
                    const string errorMessage =
                        "Data acquisition component '{componentName}' was not registered";
                    _logger.LogError(errorMessage, network);
                }
                else
                {
                    _logger.LogInformation("Config pushed to: {componentName}, config: {config}",
                        network,
                        JsonConvert.SerializeObject(notification));
                    await _componentConfigUpdateNotifier.NotifyComponentAsync(
                        dataSource.UpdateChannelName,
                        notification);
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
                var analyserComponent = await _componentRegistry
                    .GetComponentById(analyser);

                if (analyserComponent == null)
                {
                    _logger.LogWarning("Analyser {analyserName} was not registered", analyser);

                }
                else
                {
                    analysers.Add(analyserComponent);
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
