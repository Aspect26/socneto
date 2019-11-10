using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Acquisition
{
    public interface IDataAcquirer
    {
        IAsyncEnumerable<UniPost> GetPostsAsync(
            DataAcquirerInputModel jobConfig);
    }
    public interface IDataAcquirerLegacy
    {
        //Task<DataAcquirerOutputModel> AcquireBatchAsync(DataAcquirerInputModel acquirerInputModel, 
        //    CancellationToken cancellationToken);
        
    }

    


}
