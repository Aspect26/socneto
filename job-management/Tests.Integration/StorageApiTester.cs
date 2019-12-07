﻿using Domain.ComponentManagement;
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
            IEnumerable<string> assert(JobComponentConfig a, JobComponentConfig b)
            {
                yield return AssertProperty("ComponentId", (r) => r.ComponentId, a, b);
                yield return AssertProperty("JobIdd", (r) => r.JobId, a, b);
                yield return AssertProperty("OutputChannel(Length)", (r) => r.OutputMessageBrokerChannels.Length, a, b);
                yield return AssertProperty("OutputChannel(Item)", (r) => r.OutputMessageBrokerChannels[0], a, b);
                //yield return AssertProperty("Language", (r) => r.Attributes?.Count, a, b);
            }

            var componentId = $"testComponentId_{Guid.NewGuid()}";
            await InsertComponent(componentId);

            var jobId = Guid.NewGuid();
            await InsertJob(jobId);

            var jobComponent = new JobComponentConfig
            {
                 ComponentId = componentId,
                 JobId=jobId,
                 OutputMessageBrokerChannels=new[] { "channels.output1" },
                 Attributes = new Dictionary<string, JObject>()
            };
            await _componentRegistry.InsertJobComponentConfigAsync(jobComponent);

            var components = await _componentRegistry.GetAllComponentJobConfigsAsync(componentId);

            if(components.Count!=1)
            {
                throw new JmsAssertException("invalid amount of components retrieved");
            }

            var errors = assert(jobComponent, components[0]).Where(r => !(r is null));
            if (errors.Any())
            {
                var errorMessage = string.Join('\n', errors);
                _logger.LogError("Component Job Config insert failed. Errors: ", errorMessage);
                throw new JmsAssertException(errorMessage);
            }

        }

        private async Task InsertJob(Guid jobId)
        {
            IEnumerable<string> assert(Job a, Job b)
            {
                yield return AssertProperty("FinishedAt", (r) => r.FinishedAt, a, b);
                yield return AssertProperty("Job name", (r) => r.JobName, a, b);
                yield return AssertProperty("status", (r) => r.JobStatus, a, b);
                yield return AssertProperty("Language", (r) => r.Language, a, b);
                yield return AssertProperty("Owner", (r) => r.Owner, a, b);
                yield return AssertProperty("StartedAt", (r) => r.StartedAt, a, b);
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

#warning test result is not asserted
            return;

            var retrievedJob = await _jobStorage.GetJobAsync(jobId);

            var errors = assert(newJob, retrievedJob).Where(r => !(r is null));
            if (errors.Any())
            {
                var errorMessage = string.Join('\n', errors);
                _logger.LogError("Job insert failed. Errors: ", errorMessage);
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
                new Dictionary<string, JObject>()
                );

            await _componentRegistry.AddOrUpdateAsync(componentRegistrationModel);

            var retrieved = await _componentRegistry.GetComponentByIdAsync(componentId);

            var errors = assert(componentRegistrationModel, retrieved).Where(r => !(r is null));
            if (errors.Any())
            {
                var errorMessage = string.Join('\n', errors);
                _logger.LogError("Component insert failed. Errors: ", errorMessage);
                throw new JmsAssertException(errorMessage);
            }
        }

        public class JmsAssertException:Exception
        {
            public JmsAssertException(string message) : base(message)
            {
            }

            public JmsAssertException(string message, Exception innerException) : base(message, innerException)
            {
            }
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
