using Domain.EventTracking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.JobConfiguration
{
    public class DataAcquirerJobInMemoryStorage : IDataAcquirerJobStorage
    {
        private readonly ConcurrentDictionary<Guid, DataAcquirerJobConfig> _jobConfigs
            = new ConcurrentDictionary<Guid, DataAcquirerJobConfig>();
        private readonly IEventTracker<DataAcquirerJobFileStorage> _eventTracker;

        public DataAcquirerJobInMemoryStorage(
            IEventTracker<DataAcquirerJobFileStorage> eventTracker)
        {
            _eventTracker = eventTracker;
        }

        public Task<IList<DataAcquirerJobConfig>> GetAllAsync()
        {
            return Task.FromResult<IList<DataAcquirerJobConfig>>(_jobConfigs.Values.ToList());
        }

        public Task RemoveJobAsync(Guid jobId)
        {
            _jobConfigs.TryRemove(jobId, out var _);
            return Task.CompletedTask;
        }

        public Task SaveAsync(Guid jobId, DataAcquirerJobConfig jobConfig)
        {
            if (_jobConfigs.TryAdd(jobId, jobConfig))
            {
                _eventTracker.TrackWarning(
                    "AcquirerJobsAlreadyExists",
                    "The job for acquirer already exists",
                    new
                    {
                        jobId = jobId,
                        config = jobConfig
                    });
            }
            return Task.CompletedTask;
        }
    }

}
