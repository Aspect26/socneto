using System;
using System.Threading.Tasks;
using Domain.Acquisition;

namespace Domain.JobManagement
{
    public class NullMetadataStorage : IDataAcquirerMetadataStorage
    {
        public Task<IDataAcquirerMetadata> GetAsync(Guid jobId)
        {
            return Task.FromResult<IDataAcquirerMetadata>(null);
        }

        public Task SaveAsync(Guid jobId, IDataAcquirerMetadata defaultMetadata)
        {
            return Task.CompletedTask;
        }
    }




}
