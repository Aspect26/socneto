using System;
using System.Threading.Tasks;
using Domain.Acquisition;

namespace Domain.JobManagement
{
    public interface IDataAcquirerMetadataStorage
    {
        Task<IDataAcquirerMetadata> GetAsync(Guid jobId);
        Task SaveAsync(Guid jobId, IDataAcquirerMetadata defaultMetadata);
    }




}
