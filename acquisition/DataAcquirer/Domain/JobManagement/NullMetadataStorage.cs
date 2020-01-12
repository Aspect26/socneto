using System;
using System.Threading.Tasks;
using Domain.Acquisition;
using Microsoft.Extensions.Options;

namespace Domain.JobManagement
{
    public class NullMetadataStorage : IDataAcquirerMetadataStorage
    {
        public Task<T> GetAsync<T>(Guid jobId) where T : class, IDataAcquirerMetadata
        {
            return Task.FromResult((T)(IDataAcquirerMetadata) new NullMetadata());
        }

        public Task SaveAsync<T>(Guid jobId, T metadata) where T : class, IDataAcquirerMetadata
        {
            return Task.CompletedTask;
        }
    }
}
