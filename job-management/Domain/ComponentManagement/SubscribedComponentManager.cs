﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.JobStorage;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly ComponentIdentifiers _identifiers;
        private readonly IComponentRegistry _componentRegistry;
        private readonly IComponentConfigUpdateNotifier _componentConfigUpdateNotifier;
        private readonly IJobStorage _jobStorage;
        private readonly ILogger<SubscribedComponentManager> _logger;

        public SubscribedComponentManager(
            IComponentRegistry componentRegistry,
            IComponentConfigUpdateNotifier componentConfigUpdateNotifier,

            IJobStorage jobStorage,
            IOptions<ComponentIdentifiers> options,
            ILogger<SubscribedComponentManager> logger
        )
        {
            _identifiers = options.Value;
            _componentRegistry = componentRegistry;
            _componentConfigUpdateNotifier = componentConfigUpdateNotifier;
            _jobStorage = jobStorage;

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

        public async Task<JobConfigUpdateResult> StartJobAsync(
            JobConfigUpdateCommand jobConfigUpdateCommand)
        {
            var storage = _componentRegistry.GetRegisteredStorage();
            if (storage == null)
            {
                _logger.LogError("No storage component was registered");
                return JobConfigUpdateResult.Failed("No storage is present. Job can't be done");
            }


            var job = new Job
            {
                FinishedAt = null,
                JobName = jobConfigUpdateCommand.JobName,
                JobStatus = JobStatus.Running,
                JobId = jobConfigUpdateCommand.JobId,
                Owner = "admin",
                TopicQuery = jobConfigUpdateCommand.TopicQuery,
                StartedAt = DateTime.Now.Millisecond,
            };

            var analysers = await PushAnalyserJobConfig(
                storage.AnalysedDataInputChannel,
                jobConfigUpdateCommand);

            var analysersInputs = analysers.Select(r => r.InputChannelName).ToArray();
            await PushNetworkDataAcquisitionJobConfig(
                storage.AcquiredDataInputChannel,
                analysersInputs,
                jobConfigUpdateCommand);



            await _jobStorage.InsertNewJobAsync(job);

            return JobConfigUpdateResult.Successfull(
                jobConfigUpdateCommand.JobId,
                JobStatus.Running);
        }

        public async Task<JobConfigUpdateResult> StopJob(Guid jobId)
        {
            try
            {
                var job = await _jobStorage.GetJobAsync(jobId);
                var notification = new DataAcquisitionConfigUpdateNotification
                {
                    JobId = jobId,
                    Command = JobCommand.Stop.ToString()
                };

                var acquirers = job.JobComponentConfigs.Where(r => r.ComponentType == _identifiers.DataAcquirerComponentTypeName);
                foreach (var dataAcquirer in acquirers)
                {
                    await NotifyComponent(dataAcquirer.ComponentId, notification);
                }
                var analysers = job.JobComponentConfigs.Where(r => r.ComponentType == _identifiers.AnalyserComponentTypeName);
                foreach (var dataAnalyser in analysers)
                {
                    await NotifyComponent(dataAnalyser.ComponentId, notification);
                }

                job.JobStatus = JobStatus.Stopped;

                await _jobStorage.UpdateJobAsync(job);

                return JobConfigUpdateResult.Successfull(jobId, job.JobStatus);

            }
            catch (Exception e)
            {
                _logger.LogError("Could not stop the job {jobId}, due to error {error}",
                    jobId,
                    e.Message);
                throw new InvalidOperationException($"Could not stop the job {jobId}, due to error {e.Message}");
            }
        }

        private async Task PushNetworkDataAcquisitionJobConfig(
            string storageChannelName,
            IEnumerable<string> selectedAnalysersChannels,
            JobConfigUpdateCommand jobConfigUpdateCommand)
        {
            var outputChannels = selectedAnalysersChannels
                .Concat(new[] { storageChannelName, })
                .ToArray();

            foreach (var dataAcquirer in jobConfigUpdateCommand.DataAcquirers)
            {
                var attributes = jobConfigUpdateCommand
                    .Attributes
                    .GetValueOrDefault(dataAcquirer, new Dictionary<string, string>());

                attributes.Add("TopicQuery", jobConfigUpdateCommand.TopicQuery);
                attributes.Add("Language", jobConfigUpdateCommand.Language);
                var notification = new DataAcquisitionConfigUpdateNotification
                {
                    JobId = jobConfigUpdateCommand.JobId,
                    Attributes = attributes,
                    OutputMessageBrokerChannels = outputChannels,
                };

                await NotifyComponent(dataAcquirer, notification);


                //var componentConfig = new JobComponentConfig
                //{
                //    ComponentId = dataAcquirer,
                //    Attributes = attributes,
                //    ComponentType = ,
                //    InputChannelName = analyserCmp.InputChannelName,
                //    UpdateChannelName = analyserCmp.UpdateChannelName,
                //    JobId = jobConfigUpdateCommand.JobId
                //};

                //await _jobStorage.InsertJobComponentConfigAsync(componentConfig);
            }
        }

        private async Task NotifyComponent(string component, object notification)
        {
            var dataSource = await _componentRegistry.GetComponentById(component);

            if (dataSource == null)
            {
                const string errorMessage =
                    "Data acquisition component '{componentName}' was not registered";
                _logger.LogError(errorMessage, component);
            }
            else
            {
                _logger.LogInformation("Config pushed to: {componentName}, config: {config}",
                    component,
                    JsonConvert.SerializeObject(notification));
                await _componentConfigUpdateNotifier.NotifyComponentAsync(
                    dataSource.UpdateChannelName,
                    notification);
            }
        }

        private async Task<List<SubscribedComponent>> PushAnalyserJobConfig(
            string storageChannelName,

            JobConfigUpdateCommand jobConfigUpdateCommand)
        {

            var analysers = new List<SubscribedComponent>();
            foreach (var analyser in jobConfigUpdateCommand.DataAnalysers)
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
                JobId = jobConfigUpdateCommand.JobId,
                Attributes = new Dictionary<string, string>(),
                OutputMessageBrokerChannels = new[] { storageChannelName },
            };

            var configUpdateTasks = analysers.Select(async analyserCmp =>
            {
                _logger.LogInformation("Config pushed to: {componentName}, config: {config}",
                    analyserCmp,
                    JsonConvert.SerializeObject(notification));

                await _componentConfigUpdateNotifier.NotifyComponentAsync(
                    analyserCmp.UpdateChannelName,
                    notification);

                var componentConfig = new JobComponentConfig
                {
                    ComponentId = analyserCmp.ComponentId,
                    Attributes = analyserCmp
                     .Attributes
                     .ToDictionary(r => r.Key, r => (object)r.Value),
                    ComponentType = analyserCmp.ComponentType,
                    InputChannelName = analyserCmp.InputChannelName,
                    UpdateChannelName = analyserCmp.UpdateChannelName,
                    JobId = jobConfigUpdateCommand.JobId
                };

                await _jobStorage.InsertJobComponentConfigAsync(componentConfig);
            });

            await Task.WhenAll(configUpdateTasks);
            return analysers;
        }
    }
}
