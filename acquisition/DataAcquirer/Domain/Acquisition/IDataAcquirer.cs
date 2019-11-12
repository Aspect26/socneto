using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Acquisition
{


    public interface IDataAcquirerMetadata
    {

    }

    public interface IDataAcquirerMetadataContextProvider
    {
        IDataAcquirerMetadataContext Get(Guid jobId);
    }
    public interface IDataAcquirerMetadataContext
    {
        Task<T> GetOrCreateAsync<T>(T defaultIfNew) where T :IDataAcquirerMetadata;
        Task UpdateAsync(IDataAcquirerMetadata metadata);
    }


    public interface IDataAcquirer
    {
        IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
           IDataAcquirerMetadataContext context,
           DataAcquirerInputModel acquirerInputModel);
    }
    public interface IDataAcquirerLegacy
    {
        //Task<DataAcquirerOutputModel> AcquireBatchAsync(DataAcquirerInputModel acquirerInputModel, 
        //    CancellationToken cancellationToken);
        
    }

    


}
