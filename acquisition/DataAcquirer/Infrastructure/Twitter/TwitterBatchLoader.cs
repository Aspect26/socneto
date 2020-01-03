using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.Model;
using LinqToTwitter;
using Microsoft.Extensions.Logging;

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
    public class TwitterBatchLoader
    {
        private readonly Guid _jobId;
        private readonly IDataAcquirerMetadataContext _metadataContext;
        private readonly IEventTracker<TwitterBatchLoader> _logger;
        private int _foundPosts;
        private DateTime _mostRecentPost;

        public TwitterBatchLoader(
            Guid jobId,
            IDataAcquirerMetadataContext metadataContext,
            IEventTracker<TwitterBatchLoader> logger)
        {
            _jobId = jobId;
            _metadataContext = metadataContext;
            _logger = logger;
            _foundPosts = 0;
        }
        public async IAsyncEnumerable<DataAcquirerPost> CreateBatchPostEnumerator(
            TwitterContext twitterContext,
            TwitterMetadata defaultMetadata)
        {
            var includeRetweets = false;
            var metadata = await _metadataContext.GetOrCreateAsync(defaultMetadata);
            var query = defaultMetadata.Query;

            while (true)
            {
                var twitterInputModel = new TwitterQueryInput(
                    query,
                    metadata.Language,
                    metadata.MaxId,
                    metadata.SinceId,
                    metadata.BatchSize);

                var batches = QueryPastPostAsync(
                    twitterContext,
                    twitterInputModel);

                await foreach (var batch in batches)
                {
                    if (!batch.Any())
                    {
                        break;
                    }

                    foreach (var post in batch)
                    {
                        metadata.MaxId = Math.Min(post.StatusID - 1, metadata.MaxId);
                        metadata.SinceId = Math.Max(post.StatusID, metadata.SinceId);

                        if (post.CreatedAt > _mostRecentPost)
                        {
                            _mostRecentPost = post.CreatedAt;
                        }
                        if(!includeRetweets && post.RetweetedStatus.StatusID == 0)
                        {
                            yield return FromStatus(post, query);
                        }
                    }
                    _foundPosts += batch.Count;

                    _logger.TrackStatistics("QueryDataStatistics",
                        new
                        {
                            _foundPosts,
                            _mostRecentPost,
                            query = metadata.Query,
                            language = metadata.Language,
                            jobId = _jobId
                        });

                    await _metadataContext.UpdateAsync(metadata);
                }

                metadata.MaxId = ulong.MaxValue;
                await _metadataContext.UpdateAsync(metadata);

                _logger.TrackInfo("QueryData", "Waiting few minutes for new posts");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }


        private static DataAcquirerPost FromStatus(Status item, string query)
        {
            return DataAcquirerPost.FromValues(
                                item.StatusID.ToString(),
                                item.FullText,
                                item.Lang,
                                "Twitter",
                                item.User.UserID.ToString(),
                                item.CreatedAt.ToString("s"),
                                query);
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPostAsync(
          TwitterContext context,
          TwitterQueryInput acquirerInputModel)
        {
            var searchTerm = acquirerInputModel.SearchTerm;
            var language = acquirerInputModel.Language ?? "";
            var maxId = acquirerInputModel.MaxId;
            var sinceId = acquirerInputModel.SinceId;
            var batchSize = acquirerInputModel.BatchSize;

            while (true)
            {
                var batch = new List<Status>();
                try
                {

                    _logger.TrackInfo(
                       "QueryData",
                       "Downloading data");

                    var search = await GetStatusBatchAsync(context,
                        searchTerm,
                        batchSize,
                        language,
                        maxId: maxId,
                        sinceId: sinceId);

                    batch = search
                        .Statuses
                        .ToList();

                    maxId = search.Statuses.Any()
                        ? Math.Min(search.Statuses.Min(status => status.StatusID) - 1, acquirerInputModel.MaxId)
                        : acquirerInputModel.MaxId;
                }
                catch (Exception e)
                {
                    _logger.TrackError(
                        "QueryData",
                        "Unexpected error",
                        new
                        {
                            exception = e,
                            jobId = _jobId
                        });
                }

                if (batch.Count == 0)
                {
                    yield break;
                }

                yield return batch;
            }
        }



        private async Task<Search> GetStatusBatchAsync(TwitterContext context,
            string searchTerm,
            int batchSize,
            string language,
            ulong maxId = ulong.MaxValue,
            ulong sinceId = 1)
        {
            var combinedSearchResults = new List<Status>();

            // HOTFIX Null language results in exception saying that 
            // "Authorization did not succeeded". 
            if (string.IsNullOrEmpty(language))
            {
                return await context.Search
                    .Where(search => search.Type == SearchType.Search &&
                           search.Query == searchTerm &&
                           search.Count == batchSize &&
                           search.MaxID == maxId &&
                           search.SinceID == sinceId &&
                           search.TweetMode == TweetMode.Extended
                           )
                    .SingleOrDefaultAsync();
            }
            else
            {
                return await context.Search
                 .Where(search => search.Type == SearchType.Search &&
                        search.Query == searchTerm &&
                        search.SearchLanguage == language &&
                        search.Count == batchSize &&
                        search.TweetMode == TweetMode.Extended &&
                        search.MaxID == maxId &&
                        search.SinceID == sinceId)
                 .SingleOrDefaultAsync();
            }
        }




    }
}
