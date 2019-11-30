using System;
using System.Threading.Tasks;
using Domain.Acquisition;

namespace Domain.JobManagement
{
    public class NullMetadataStorage : IDataAcquirerMetadataStorage
    {

        Task<T> IDataAcquirerMetadataStorage.GetAsync<T>(Guid jobId)
        {

            return Task.FromResult((T)(IDataAcquirerMetadata) new NullMetadata());
        }

        Task IDataAcquirerMetadataStorage.SaveAsync<T>(T metadata)
        {
            return Task.CompletedTask;
        }
    }




}
