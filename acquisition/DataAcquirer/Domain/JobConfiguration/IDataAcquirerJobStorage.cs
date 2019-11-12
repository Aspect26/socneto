using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.JobConfiguration
{
    public interface IDataAcquirerJobStorage
    {
        Task<IList<DataAcquirerJobConfig>> GetAllAsync();

        Task RemoveJobAsync(Guid jobId);

        Task SaveAsync(Guid jobId, DataAcquirerJobConfig jobConfig);

    }

}
