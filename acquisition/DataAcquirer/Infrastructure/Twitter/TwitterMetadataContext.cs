using Domain.Acquisition;
using Domain.JobManagement;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class TwitterMetadataContext : IDataAcquirerMetadataContext
    {
        private readonly Guid _jobId;
        private readonly IDataAcquirerMetadataStorage _jobMetadataStorage;

        public TwitterMetadataContext(Guid jobId,

            IDataAcquirerMetadataStorage jobMetadataStorage)
        {
            _jobId = jobId;
            _jobMetadataStorage = jobMetadataStorage;
        }
        public async Task<T> GetOrCreateAsync<T>(T defaultMetadata) where T : class, IDataAcquirerMetadata
        {
            var metadata = await _jobMetadataStorage.GetAsync<T>(_jobId);
            if (metadata == null)
            {
                await _jobMetadataStorage.SaveAsync(_jobId, defaultMetadata);
                return defaultMetadata;
            }

            return metadata;
        }

        public async Task UpdateAsync<T>(T metadata) where T : class, IDataAcquirerMetadata
        {
            await _jobMetadataStorage.SaveAsync(_jobId, metadata);
        }
    }
}
