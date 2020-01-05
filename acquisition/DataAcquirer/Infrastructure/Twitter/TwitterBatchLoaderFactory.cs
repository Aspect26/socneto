using System;
using Domain;
using Domain.Acquisition;
using Infrastructure.Twitter.Abstract;
using Microsoft.Extensions.Options;

namespace Infrastructure.Twitter
{
    public class TwitterBatchLoaderFactory
    {
        private readonly IDataAcquirerMetadataContextProvider _dataAcquirerMetadataContextProvider;
        private readonly IOptions<TwitterBatchLoaderOptions> _batchLoaderOptions;
        private readonly IEventTracker<TwitterBatchLoader> _childEventTracker;

        public TwitterBatchLoaderFactory(
            IOptions<TwitterBatchLoaderOptions> batchLoaderOptions,
            IEventTracker<TwitterBatchLoader> childEventTracker,
            IDataAcquirerMetadataContextProvider dataAcquirerMetadataContextProvider)
        {
            _batchLoaderOptions = batchLoaderOptions;
            _childEventTracker = childEventTracker;
            _dataAcquirerMetadataContextProvider = dataAcquirerMetadataContextProvider;
        }

        public TwitterBatchLoader Create(Guid jobId)
        {
            var context = _dataAcquirerMetadataContextProvider.Get(jobId);
            return new TwitterBatchLoader(
                jobId,
                context,
                _batchLoaderOptions,
                _childEventTracker);
        }
    }
}
