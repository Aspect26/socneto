using System;

namespace Domain.Acquisition
{
    public class NullContextProvider : IDataAcquirerMetadataContextProvider
    {
        public IDataAcquirerMetadataContext Get(Guid jobId)
        {
            return new NullContext();
        }
    }

    


}
