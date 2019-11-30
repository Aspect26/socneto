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

        public TwitterMetadataContext(Guid jobId, IDataAcquirerMetadataStorage jobMetadataStorage)
        {
            _jobId = jobId;
            _jobMetadataStorage = jobMetadataStorage;
        }
        public async Task<T> GetOrCreateAsync<T>(T defaultMetadata) where T : class,IDataAcquirerMetadata
        {
            var metadata = await _jobMetadataStorage.GetAsync<T>(_jobId);
            if (metadata == null)
            {
                await _jobMetadataStorage.SaveAsync(defaultMetadata);
                return defaultMetadata;
            }

            if (metadata is T typed)
            {
                return typed;
            }
            else
            {
                throw new InvalidOperationException("Type of metadata does not correspond to retreived type");
            }
        }

        public async Task UpdateAsync(IDataAcquirerMetadata metadata)
        {
            await _jobMetadataStorage.SaveAsync(metadata);
        }
    }
}
