using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.Model;
using Infrastructure.Twitter.Abstract;
using LinqToTwitter;
using Microsoft.Extensions.Options;

namespace Infrastructure.Twitter
{
    public class TwitterBatchLoader
    {
        private readonly Guid _jobId;
        private readonly IDataAcquirerMetadataContext _metadataContext;
        private readonly IEventTracker<TwitterBatchLoader> _logger;
        private readonly TwitterBatchLoaderOptions _options;
        private int _foundPosts;
        private DateTime _mostRecentPost;

        public TwitterBatchLoader(
            Guid jobId,
            IDataAcquirerMetadataContext metadataContext,
            IOptions<TwitterBatchLoaderOptions> optionsAccessor,
            IEventTracker<TwitterBatchLoader> logger)
        {
            _jobId = jobId;
            _metadataContext = metadataContext;
            _logger = logger;
            _options = optionsAccessor.Value;
            _foundPosts = 0;
        }
        public async IAsyncEnumerable<DataAcquirerPost> CreateBatchPostEnumerator(
            ITwitterContext twitterContext,
            TwitterMetadata defaultMetadata)
        {
            var includeRetweets = false;
            var metadata = await _metadataContext.GetOrCreateAsync(defaultMetadata);
            _logger.TrackInfo("QueryData", "Twitter - using metadata",
                new
                {
                    metadata,
                    jobId = _jobId
                });
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
                        var isRetweet = post.RetweetedStatus?.StatusID > 0;
                        if (isRetweet && !includeRetweets)
                        {
                            continue;
                        }

                        yield return FromStatus(post, query);

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

                _logger.TrackInfo("QueryData", $"Waiting {_options.NoPostWaitDelay} for new posts");
                await Task.Delay(_options.NoPostWaitDelay);
            }
        }


        private static DataAcquirerPost FromStatus(Status item, string query)
        {
            return DataAcquirerPost.FromValues(
                                item.StatusID.ToString(),
                                item.FullText,
                                item.Lang,
                                "twitter",
                                item.User.ScreenNameResponse,
                                item.CreatedAt.ToString("s"),
                                query);
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPostAsync(
          ITwitterContext context,
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

                    var statuses = await context.GetStatusBatchAsync(
                        searchTerm,
                        batchSize,
                        language,
                        maxId: maxId,
                        sinceId: sinceId);
                    
                    batch = statuses
                        .ToList();

                    maxId = statuses.Any()
                        ? Math.Min(statuses.Min(status => status.StatusID) - 1, acquirerInputModel.MaxId)
                        : acquirerInputModel.MaxId;
                }
                catch (TwitterQueryException e) when (e.ErrorCode == 88)
                {
                    Console.WriteLine("Rate limit exceeded - waiting");
                    await Task.Delay(_options.RateLimitExceededWaitDelay);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.TrackError(
                        "QueryData",
                        "Unexpected error",
                        new
                        {
                            exception = e,
                            jobId = _jobId,
                            input = acquirerInputModel
                        });
                    await Task.Delay(_options.ErrorEncounteredWaitDelay);
                    continue;
                }

                if (batch.Count == 0)
                {
                    yield break;
                }

                yield return batch;
            }
        }






    }
}
