using Domain.ComponentManagement;
using Domain.JobStorage;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Integration
{
    public class StorageApiTester
    {
        private readonly IJobStorage _jobStorage;
        private readonly IComponentRegistry _componentRegistry;
        private readonly ILogger<StorageApiTester> _logger;

        public StorageApiTester(
            IJobStorage jobStorage,
            IComponentRegistry componentRegistry,
            ILogger<StorageApiTester> logger)
        {
            _jobStorage = jobStorage;
            _componentRegistry = componentRegistry;
            _logger = logger;
        }

        // TODO add attributes


        public async Task TestAsync()
        {
            var componentId = $"testComponentId_{Guid.NewGuid()}";
            await InsertComponent(componentId);
            

            var jobId = Guid.NewGuid();
            await InsertJob(jobId);

            await InsertComponentJobConfigAsync(componentId, jobId);            
        }

        private async Task InsertComponentJobConfigAsync(string componentId, Guid jobId)
        {
            IEnumerable<string> assert(JobComponentConfig a, JobComponentConfig b)
            {
                yield return AssertProperty("ComponentId", (r) => r.ComponentId, a, b);
                yield return AssertProperty("JobId", (r) => r.JobId, a, b);
                yield return AssertProperty("OutputChannel(Length)", (r) => r.OutputMessageBrokerChannels.Length, a, b);
                yield return AssertProperty("OutputChannel(Item)", (r) => r.OutputMessageBrokerChannels[0], a, b);
                yield return AssertProperty("Attributes", (r) => r.Attributes?.Count, a, b);
            }

            var jobComponent = new JobComponentConfig
            {
                ComponentId = componentId,
                JobId = jobId,
                OutputMessageBrokerChannels = new[] { "channels.output1" },
                Attributes = new Dictionary<string, JObject>()
            };
            await _componentRegistry.InsertJobComponentConfigAsync(jobComponent);

            var components = await _componentRegistry.GetAllComponentJobConfigsAsync(componentId);

            if (components.Count != 1)
            {
                throw new JmsAssertException("invalid amount of components retrieved");
            }

            var errors = assert(jobComponent, components[0]).Where(r => !(r is null));
            
            AssertErrors("Component Job config", errors);
        }

        private async Task InsertJob(Guid jobId)
        {
            IEnumerable<string> assert(Job a, Job b)
            {
                yield return AssertProperty("FinishedAt", (r) => r.FinishedAt, a, b);
                yield return AssertProperty("Job name", (r) => r.JobName, a, b);
                yield return AssertProperty("status", (r) => r.JobStatus, a, b);
                yield return AssertProperty("Language", (r) => r.Language, a, b);
              
                
                //yield return AssertProperty("Owner", (r) => r.Owner, a, b);
                
                // removed due to utc fuck up
                //yield return AssertProperty("StartedAt", (r) => r.StartedAt, a, b);
                yield return AssertProperty("Topic query", (r) => r.TopicQuery, a, b);
            }

            var newJob = new Job
            {
                FinishedAt = null,
                JobName = "test_job_jk",
                JobStatus = JobStatus.Running,
                JobId = jobId,
                Owner = "admin",
                TopicQuery = "some topic",
                StartedAt = DateTime.Now,
            };
            await _jobStorage.InsertNewJobAsync(newJob);

            var retrievedJob = await _jobStorage.GetJobAsync(jobId);

            var errors = assert(newJob, retrievedJob).Where(r => !(r is null));
            AssertErrors("Job insert", errors);
            
            var toBeUpdatedJob = new Job
            {
                FinishedAt = null,
                JobName = "test_job_jk_foo_x",
                JobStatus = JobStatus.Stopped,
                JobId = jobId,
                Owner = "admin",
                TopicQuery = "some topic",
                StartedAt = DateTime.Now,
            };

            await _jobStorage.InsertNewJobAsync(toBeUpdatedJob);

            var updatedJob = await _jobStorage.GetJobAsync(jobId);
            var errorsAfterUpdate = assert(toBeUpdatedJob, updatedJob);
            
            AssertErrors( "JobUpdate", errors);
        }

        private void AssertErrors(string name,IEnumerable<string> errors)
        {
            if (errors.Where(r=>!(r is null)).Any())
            {
                var errorMessage = string.Join('\n', errors);
                _logger.LogError($"{name} failed. Errors: ", errorMessage);
                throw new JmsAssertException(errorMessage);
            }
        }

        private async Task InsertComponent(string componentId)
        {
            
            IEnumerable<string> assert(ComponentModel a, ComponentModel b)
            {
                yield return AssertProperty("Type", (r) => r.ComponentType, a, b);
                yield return AssertProperty("Input channel", (r) => r.InputChannelName, a, b);
                yield return AssertProperty("Update channel", (r) => r.UpdateChannelName, a, b);
                yield return AssertProperty("Attribute count", (r) => r.Attributes.Count, a, b);
                
            }
            _logger.LogInformation("Starting testing storage api");

            
            var componentRegistrationModel = new ComponentModel(
                componentId,
                "DATA_ANALYSER",
                "channel.input",
                "channel.update",
                new Dictionary<string, JObject>
                {
                    {
                        "AnalyserFormat",
                        JObject.FromObject(new
                        {
                            Foo="Bar",
                            Baz="Boo"
                        }) }
                }
                );

            await _componentRegistry.AddOrUpdateAsync(componentRegistrationModel);

            var retrieved = await _componentRegistry.GetComponentByIdAsync(componentId);

            var errors = assert(componentRegistrationModel, retrieved);
            AssertErrors("Component insert", errors);
        }

        public string AssertProperty<TObj, TReturn>(
            string propertyName,
            Func<TObj, TReturn> propertySelector,
            TObj a,
            TObj b)
        {
            var assertErrorMessage = "Different {0} a: {1} b: {2}";
            if (propertySelector(a)?.ToString() != propertySelector(b)?.ToString())
                return string.Format(
                    assertErrorMessage,
                    propertyName,
                    propertySelector(a),
                    propertySelector(b));
            return null;
        }

    }
}
