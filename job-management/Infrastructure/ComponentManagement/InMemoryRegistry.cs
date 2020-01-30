using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ComponentManagement
{
    public class InMemoryRegistry : IComponentRegistry
    {
        private readonly ConcurrentDictionary<string, ComponentModel> _components
            = new ConcurrentDictionary<string, ComponentModel>();

        private readonly ConcurrentDictionary<string, List<JobComponentConfig>> _jobComponentConfigs
            = new ConcurrentDictionary<string, List<JobComponentConfig>>();

        private readonly StorageChannelNames _storageChannelNames;
        private readonly ILogger<InMemoryRegistry> _logger;

        public InMemoryRegistry(
            IOptions<StorageChannelNames> storageChannelNamesAccessor,
            ILogger<InMemoryRegistry> logger)
        {
            _storageChannelNames = storageChannelNamesAccessor.Value;
            _logger = logger;
        }

        public Task AddOrUpdateAsync(ComponentModel componentRegistrationModel)
        {
            var key = componentRegistrationModel.ComponentId;
            _components.AddOrUpdate(key, componentRegistrationModel, (k, v) => componentRegistrationModel);
            return Task.CompletedTask;
        }

        public Task<ComponentModel> GetComponentByIdAsync(string componentId)
        {
            if (_components.TryGetValue(componentId, out var value))
            {
                return Task.FromResult(value);
            }
            return Task.FromResult<ComponentModel>(null);
        }

        public Task<List<JobComponentConfig>> GetAllComponentJobConfigsAsync(string componentId)
        {
            if (_jobComponentConfigs.TryGetValue(componentId, out var configs))
            {
                return Task.FromResult(configs.ToList());
            }
            return Task.FromResult(new List<JobComponentConfig>());
        }

        public Task InsertJobComponentConfigAsync(JobComponentConfig jobConfig)
        {
            _jobComponentConfigs.AddOrUpdate(
                jobConfig.ComponentId,
                new List<JobComponentConfig> { jobConfig },
                (key, list) =>
                {
                    list.Add(jobConfig);
                    return list;
                });
            return Task.CompletedTask;
        }

        public StorageComponent GetRegisteredStorage()
        {
            return new StorageComponent
            {
                AnalysedDataInputChannel = _storageChannelNames.StoreAnalysedDataChannelName,
                AcquiredDataInputChannel = _storageChannelNames.StoreRawDataChannelName
            };
        }

        public Task<List<ComponentModel>> GetAllComponentsAsync()
        {
            return Task.FromResult(_components.Values.ToList());
        }
    }
}