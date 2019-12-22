using Domain;
using Domain.Acquisition;
using Domain.Model;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class TwitterDataAcquirer : IDataAcquirer
    {
        private readonly TwitterContextProvider _twitterContextProvider;
        private readonly IEventTracker<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            TwitterContextProvider twitterContextProvider,
            IEventTracker<TwitterDataAcquirer> logger)
        {
            _twitterContextProvider = twitterContextProvider;
            _logger = logger;
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            IDataAcquirerMetadataContext context,
            DataAcquirerInputModel acquirerInputModel)
        {
            var credentials = new TwitterCredentials
            {
                ConsumerKey = acquirerInputModel.Attributes.GetValue("ApiKey"),
                ConsumerSecret = acquirerInputModel.Attributes.GetValue("ApiSecretKey"),
                AccessToken = acquirerInputModel.Attributes.GetValue("AccessToken"),
                AccessTokenSecret = acquirerInputModel.Attributes.GetValue("AccessTokenSecret")
            };

            var parsedTwitterQuery = ParseTwitterQuery(acquirerInputModel.Query);

            var defaultMetadata = new TwitterMetadata
            {
                Credentials = credentials,
                MaxId = ulong.MaxValue,
                SinceId = 0,
                //Language = acquirerInputModel.Attributes.GetValue("Language", "en"),
                Query = parsedTwitterQuery,
                BatchSize = acquirerInputModel.BatchSize
            };

            var metadata = await context.GetOrCreateAsync(defaultMetadata);

            var twitterContext =await  _twitterContextProvider.GetContextAsync(credentials);

            long foundPosts = 0;
            var mostRecentPost = DateTime.MinValue;
            while (true)
            {
                var twitterInputModel = new TwitterQueryInput(
                    metadata.Query,
                    metadata.Language,
                    metadata.MaxId,
                    metadata.SinceId,
                    metadata.BatchSize);

                var batches = QueryPastPostAsync(
                    twitterContext,
                    acquirerInputModel.JobId, 
                    twitterInputModel);

                await foreach (var batch in batches)
                {
                    if (!batch.Any())
                    {
                        _logger.TrackWarning(
                            "QueryData",
                            "Querying yielded empty batch",
                            new
                            {
                                query = metadata.Query,
                                jobId = acquirerInputModel.JobId
                            });
                        break;
                    }


                    foreach (var post in batch)
                    {
                        metadata.MaxId = Math.Min(post.StatusID - 1, metadata.MaxId);
                        metadata.SinceId = Math.Max(post.StatusID, metadata.SinceId);

                        if (post.CreatedAt > mostRecentPost)
                        {
                            mostRecentPost = post.CreatedAt;
                        }
                        yield return FromStatus(post);
                    }
                    foundPosts += batch.Count;

                    _logger.TrackStatistics("QueryDataStatistics",
                        new
                        {
                            foundPosts,
                            mostRecentPost,
                            query = metadata.Query,
                            language = metadata.Language,
                            jobId = acquirerInputModel.JobId
                        });

                    await context.UpdateAsync(metadata);
                }

                metadata.MaxId = ulong.MaxValue;
                await context.UpdateAsync(metadata);
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }

        private string ParseTwitterQuery(string query)
        {
            return query
                .Replace("NOT ", "-")
                .Replace(" AND ", " ");
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPostAsync(
            TwitterContext context,
            Guid jobId,
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

                    var search = await GetStatusBatchAsync( context,
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
                            jobId = jobId
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

            return await context.Search
                .Where(search => search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       // search.SearchLanguage == language &&
                       search.Count == batchSize &&
                       search.MaxID == maxId &&
                       search.SinceID == sinceId)
                .SingleOrDefaultAsync();
        }



        private static DataAcquirerPost FromStatus(Status item)
        {
            return DataAcquirerPost.FromValues(
                                item.StatusID.ToString(),
                                item.Text ?? item.FullText,
                                item.Lang,
                                "Twitter",
                                item.User.UserID.ToString(),
                                item.CreatedAt.ToString("s"));
        }
    }
}
