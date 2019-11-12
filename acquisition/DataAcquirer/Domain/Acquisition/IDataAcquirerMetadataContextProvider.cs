using System;

namespace Domain.Acquisition
{
    public interface IDataAcquirerMetadataContextProvider
    {
        IDataAcquirerMetadataContext Get(Guid jobId);
    }

    


}
