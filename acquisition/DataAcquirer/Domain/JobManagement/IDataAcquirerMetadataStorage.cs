using System;
using System.Threading.Tasks;
using Domain.Acquisition;

namespace Domain.JobManagement
{
    public interface IDataAcquirerMetadataStorage
    {
        Task<T> GetAsync<T>(Guid jobId) where T : class, IDataAcquirerMetadata;
        Task SaveAsync<T>(Guid jobId,T metadata) where T : class, IDataAcquirerMetadata;
    }

}
