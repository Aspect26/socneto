using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.EventTracking;
using Domain.JobStorage;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly ComponentIdentifiers _identifiers;
        private readonly IComponentRegistry _componentRegistry;
        private readonly IComponentConfigUpdateNotifier _componentConfigUpdateNotifier;
        private readonly IJobStorage _jobStorage;
        private readonly IEventTracker<SubscribedComponentManager> _logger;

        public SubscribedComponentManager(
            IComponentRegistry componentRegistry,
            IComponentConfigUpdateNotifier componentConfigUpdateNotifier,

            IJobStorage jobStorage,
            IOptions<ComponentIdentifiers> options,
            IEventTracker<SubscribedComponentManager> logger)
        {
            _identifiers = options.Value;
            _componentRegistry = componentRegistry;
            _componentConfigUpdateNotifier = componentConfigUpdateNotifier;
            _jobStorage = jobStorage;

            _logger = logger;
        }

        public async Task<SubscribedComponentResultModel> SubscribeComponentAsync(
            ComponentModel componentRegistrationModel)
        {
            try
            {
                await _componentRegistry.AddOrUpdateAsync(componentRegistrationModel);

                return SubscribedComponentResultModel.Successful();
            }
            catch (Exception e)
            {
                string error = $"Subscription failed due to: {e.Message}";
                _logger.TrackError("SubscribeComponent", error, new { componentRegistrationModel, exception = e });
                return SubscribedComponentResultModel.Failed(error);
            }
        }

        public async Task<JobConfigUpdateResult> StartJobAsync(
            JobConfigUpdateCommand jobConfigUpdateCommand)
        {
            var storage = _componentRegistry.GetRegisteredStorage();
            if (storage == null)
            {
                _logger.TrackError(
                    "StartJob",
                    "No storage component was registered");
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
                StartedAt = DateTime.Now,
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
                    Command = JobCommand.Stop
                };

                var components = await _componentRegistry.GetAllComponentsAsync();

                foreach (var item in components)
                {
                    await NotifyComponent(jobId, item.ComponentId, notification);
                }

                job.JobStatus = JobStatus.Stopped;

                await _jobStorage.UpdateJobAsync(job);

                return JobConfigUpdateResult.Successfull(jobId, job.JobStatus);
            }
            catch (Exception e)
            {
                var message = $"Could not stop the job {jobId}, due to error {e.Message}";

                _logger.TrackError(
                    "StopJob",
                    message,
                    new
                    {
                        jobId,
                        exception = e
                    });

                throw new InvalidOperationException(message);
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
                    .Attributes.GetValue(dataAcquirer)?.ToObject<JObject>() ?? new JObject();

                try
                {
                    var topicQuery = new JProperty("TopicQuery", jobConfigUpdateCommand.TopicQuery);
                    attributes.Add(topicQuery);
                    var languageProperty = new JProperty("Language", jobConfigUpdateCommand.Language);
                    attributes.Add(languageProperty);
                }
                catch (Exception e)
                {
                    _logger.TrackError(
                   "PushNetworkJobConfig",
                   "Error while adding attributes",
                   new
                   {
                       jobId = jobConfigUpdateCommand.JobId,
                       exception = e
                   });

                    throw;
                }

                var notification = new DataAcquisitionConfigUpdateNotification
                {
                    JobId = jobConfigUpdateCommand.JobId,
                    Attributes = attributes,
                    OutputMessageBrokerChannels = outputChannels,
                    Command = JobCommand.Start
                };

                await NotifyComponent(
                    jobConfigUpdateCommand.JobId,
                    dataAcquirer,
                    notification);

                var componentConfig = new JobComponentConfig
                {
                    ComponentId = dataAcquirer,
                    Attributes = attributes,
                    JobId = jobConfigUpdateCommand.JobId,
                    OutputMessageBrokerChannels = notification.OutputMessageBrokerChannels
                };

                await _componentRegistry.InsertJobComponentConfigAsync(componentConfig);
            }
        }

        private async Task NotifyComponent(Guid jobId, string component, object notification)
        {
            var dataSource = await _componentRegistry.GetComponentByIdAsync(component);

            if (dataSource == null)
            {
                var errorMessage =
                    $"Data acquisition component '{component}' was not registered";

                _logger.TrackError(
                  "NotifyComponent",
                  errorMessage);
            }
            else
            {
                var metrics = $"Config pushed to: {component}";
                _logger.TrackInfo(
                  "NotifyComponent",
                  metrics,
                  new
                  {
                      jobId,
                      config = notification
                  });
                await _componentConfigUpdateNotifier.NotifyComponentAsync(
                    dataSource.UpdateChannelName,
                    notification);
            }
        }

        private async Task<List<ComponentModel>> PushAnalyserJobConfig(
            string storageChannelName,

            JobConfigUpdateCommand jobConfigUpdateCommand)
        {

            var analysers = new List<ComponentModel>();
            foreach (var analyser in jobConfigUpdateCommand.DataAnalysers)
            {
                var analyserComponent = await _componentRegistry
                    .GetComponentByIdAsync(analyser);

                if (analyserComponent == null)
                {
                    _logger.TrackWarning(
                        "ComponentConfig",
                        $"Analyser {analyser} was not registered");
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
                _logger.TrackInfo(
                    "ComponentConfig",
                    $"Config pushed to: {analyserCmp}, updateChannelName: {analyserCmp.UpdateChannelName}",
                    new
                    {
                        config = notification
                    });

                await _componentConfigUpdateNotifier.NotifyComponentAsync(
                    analyserCmp.UpdateChannelName,
                    notification);

                var componentConfig = new JobComponentConfig
                {
                    ComponentId = analyserCmp.ComponentId,
                    Attributes = analyserCmp.Attributes,
                    JobId = jobConfigUpdateCommand.JobId,
                    OutputMessageBrokerChannels = notification.OutputMessageBrokerChannels
                };

                await _componentRegistry.InsertJobComponentConfigAsync(componentConfig);

            });

            await Task.WhenAll(configUpdateTasks);
            return analysers;
        }
    }
}
