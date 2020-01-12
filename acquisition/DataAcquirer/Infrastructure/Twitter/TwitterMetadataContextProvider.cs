using Domain.Acquisition;
using Domain.JobManagement;
using System;

namespace Infrastructure.Twitter
{
    public class TwitterMetadataContextProvider : IDataAcquirerMetadataContextProvider
    {
        private readonly IDataAcquirerMetadataStorage _storage;

        public TwitterMetadataContextProvider(IDataAcquirerMetadataStorage storage)
        {
            _storage = storage;
        }
        public IDataAcquirerMetadataContext Get(Guid jobId)
        {
            return new TwitterMetadataContext(jobId, _storage);
        }

       
    }
}
