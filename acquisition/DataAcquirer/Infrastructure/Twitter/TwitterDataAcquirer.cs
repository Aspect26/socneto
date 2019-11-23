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
        private TwitterContext _twitterContext;
        private readonly IEventTracker<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            IEventTracker<TwitterDataAcquirer> logger)
        {
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

            var defaultMetadata = new TwitterMetadata
            {
                Credentials = credentials,
                MaxId = ulong.MaxValue,
                SinceId = 0,
                Language = acquirerInputModel.Attributes.GetValue("Language", "en"),
                Query = acquirerInputModel.Query,
                BatchSize = acquirerInputModel.BatchSize
            };

            var metadata = await context.GetOrCreateAsync(defaultMetadata);

            if (_twitterContext == null)
            {
                // TODO support multiuser
                // TODO use singleton object handling this
                CreateContext(credentials);
            }

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

                var batches = QueryPastPostAsync(acquirerInputModel.JobId, twitterInputModel);

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
                            query = acquirerInputModel.Query,
                            language = acquirerInputModel.QueryLanguage,
                            jobId = acquirerInputModel.JobId
                        });

                    await context.UpdateAsync(metadata);
                }

                metadata.MaxId = ulong.MaxValue;
                await context.UpdateAsync(metadata);
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPostAsync(
            Guid jobId,
            TwitterQueryInput acquirerInputModel)
        {
            var searchTerm = acquirerInputModel.SearchTerm;
            var language = acquirerInputModel.Language;
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

                    var search = await GetStatusBatchAsync(searchTerm, batchSize, language, maxId: maxId, sinceId: sinceId);

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



        private async Task<Search> GetStatusBatchAsync(
            string searchTerm,
            int batchSize,
            string language,
            ulong maxId = ulong.MaxValue,
            ulong sinceId = 1)
        {
            var combinedSearchResults = new List<Status>();

            return await _twitterContext.Search
                .Where(search => search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.SearchLanguage == language &&
                       search.Count == batchSize &&
                       search.MaxID == maxId &&
                       search.SinceID == sinceId &&
                       search.TweetMode == TweetMode.Compat)
                .SingleOrDefaultAsync();
        }

        private void CreateContext(TwitterCredentials credentials)
        {
            try
            {

                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = credentials.ConsumerKey,
                        ConsumerSecret = credentials.ConsumerSecret,
                        AccessToken = credentials.AccessToken,
                        AccessTokenSecret = credentials.AccessTokenSecret
                    }
                };
                _twitterContext = new TwitterContext(auth);
            }
            catch (Exception e)
            {
                _logger.TrackError(
                    "QueryData",
                    "Twitter context failed to initialize",
                    new
                    {
                        exception = e
                    });
                throw;
            }
        }

        private static DataAcquirerPost FromStatus(Status item)
        {
            return DataAcquirerPost.FromValues(
                                item.StatusID.ToString(),
                                item.Text ?? item.FullText,
                                "Twitter",
                                item.User.UserID.ToString(),
                                item.CreatedAt.ToString("s"));
        }
    }
}
