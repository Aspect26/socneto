using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Job;
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
        private readonly IJobConfigStorage _jobConfigStorage;
        private readonly IJobStorage _jobStorage;
        private readonly ILogger<SubscribedComponentManager> _logger;

        public SubscribedComponentManager(
            IComponentRegistry componentRegistry,
            IComponentConfigUpdateNotifier componentConfigUpdateNotifier,
            IJobConfigStorage jobConfigStorage,
            IJobStorage jobStorage,
            ILogger<SubscribedComponentManager> logger
        )
        {
            _componentRegistry = componentRegistry;
            _componentConfigUpdateNotifier = componentConfigUpdateNotifier;
            _jobConfigStorage = jobConfigStorage;
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

            var analysers = await PushAnalyserJobConfig(
                storage.AnalysedDataInputChannel,
                jobConfigUpdateCommand);

            var analysersInputs = analysers.Select(r => r.InputChannelName).ToArray();
            await PushNetworkDataAcquisitionJobConfig(
                storage.AcquiredDataInputChannel,
                analysersInputs,
                jobConfigUpdateCommand);

            var jobConfig = new JobConfig
            {
                JobStatus = JobStatus.Running,
                JobId = jobConfigUpdateCommand.JobId,
                TopicQuery = jobConfigUpdateCommand.TopicQuery,
                DataAnalysers = jobConfigUpdateCommand.DataAnalysers.ToList(),
                DataAcquirers = jobConfigUpdateCommand.DataAcquirers.ToList()
            };

            // TODO: get the owner somehow
            var job = new Models.Job
            {
                JobId = jobConfigUpdateCommand.JobId,
                JobName = jobConfigUpdateCommand.JobName,
                Owner = "admin",
                HasFinished = false,
                StartedAt = DateTime.Now
            };

            await _jobStorage.InsertNewJobAsync(job);
            await _jobConfigStorage.InsertNewJobConfigAsync(jobConfig);

            return JobConfigUpdateResult.Successfull(
                jobConfigUpdateCommand.JobId,
                JobStatus.Running);
        }

        public async Task<JobConfigUpdateResult> StopJob(Guid jobId)
        {
            try
            {
                var jobConfig = await _jobConfigStorage.GetJobConfigAsync(jobId);
                var notification = new DataAcquisitionConfigUpdateNotification
                {
                    JobId = jobId,
                    Command = JobCommand.Stop.ToString()
                };

                foreach (var dataAcquirer in jobConfig.DataAcquirers)
                {
                    await NotifyComponent(dataAcquirer, notification);
                }
                foreach (var dataAnalyser in jobConfig.DataAnalysers)
                {
                    await NotifyComponent(dataAnalyser, notification);
                }
                
                jobConfig.JobStatus = JobStatus.Stopped;

                await _jobConfigStorage.UpdateJobConfig(jobConfig);

                return JobConfigUpdateResult.Successfull(jobId, jobConfig.JobStatus);

            }
            catch (Exception e)
            {
                _logger.LogError("Could not stop the job {jobId}, due to error {error}", 
                    jobId,
                    e.Message);
                throw  new InvalidOperationException($"Could not stop the job {jobId}, due to error {e.Message}");
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

            var notification = new DataAcquisitionConfigUpdateNotification
            {
                JobId = jobConfigUpdateCommand.JobId,
                Attributes = new Dictionary<string, string>()
                {
                    {"TopicQuery", jobConfigUpdateCommand.TopicQuery }
                },
                OutputMessageBrokerChannels = outputChannels,
            };

            foreach (var dataAcquirer in jobConfigUpdateCommand.DataAcquirers)
            {
                await NotifyComponent(dataAcquirer, notification);
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
