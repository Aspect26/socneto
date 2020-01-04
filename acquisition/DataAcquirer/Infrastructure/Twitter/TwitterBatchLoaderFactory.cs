using System;
using Domain;
using Domain.Acquisition;

namespace Infrastructure.Twitter
{
    public class TwitterBatchLoaderFactory
    {
        private readonly IDataAcquirerMetadataContextProvider _dataAcquirerMetadataContextProvider;
        private readonly IEventTracker<TwitterBatchLoader> _childEventTracker;

        public TwitterBatchLoaderFactory(
            IEventTracker<TwitterBatchLoader> childEventTracker,
            IDataAcquirerMetadataContextProvider dataAcquirerMetadataContextProvider)
        {
            _childEventTracker = childEventTracker;
            _dataAcquirerMetadataContextProvider = dataAcquirerMetadataContextProvider;
        }

        public TwitterBatchLoader Create(Guid jobId)
        {
            var context = _dataAcquirerMetadataContextProvider.Get(jobId);
            return new TwitterBatchLoader(
                jobId,
                context,
                _childEventTracker);
        }
    }
}
