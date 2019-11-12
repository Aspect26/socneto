using Domain.Acquisition;
using Domain.Model;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{

    public class TwitterDataAcquirer : IDataAcquirer
    {
        private TwitterContext _twitterContext;
        private readonly ILogger<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            ILogger<TwitterDataAcquirer> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            IDataAcquirerMetadataContext context,
            DataAcquirerInputModel acquirerInputModel)
        {
            var credentials = new TwitterCredentials
            {
                ConsumerKey = acquirerInputModel.Attributes["ApiKey"],
                ConsumerSecret = acquirerInputModel.Attributes["ApiSecretKey"],
                AccessToken = acquirerInputModel.Attributes["AccessToken"],
                AccessTokenSecret = acquirerInputModel.Attributes["AccessTokenSecret"]
            };

            var defaultMetadata = new TwitterMetadata
            {
                Credentials = credentials,
                MaxId = ulong.MaxValue,
                SinceId = 0,
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

            while (true)
            {
                var twitterInputModel = new TwitterQueryInput(
                    metadata.Query,
                    metadata.MaxId,
                    metadata.SinceId,
                    metadata.BatchSize);

                var batches = QueryPastPost(twitterInputModel);

                await foreach (var batch in batches)
                {
                    if (!batch.Any())
                    {
                        _logger.LogWarning("Querying yielded empty batch");

                        break;
                    }
                    foreach (var post in batch)
                    {
                        metadata.MaxId = Math.Min(post.StatusID - 1, metadata.MaxId);
                        metadata.SinceId = Math.Max(post.StatusID, metadata.SinceId);

                        yield return FromStatus(post);
                    }

                    await context.UpdateAsync(metadata);
                }

                // todo update sinceId, maxId and do it again

                metadata.MaxId = ulong.MaxValue;
                await context.UpdateAsync(metadata);
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPost(TwitterQueryInput acquirerInputModel)
        {
            var searchTerm = acquirerInputModel.SearchTerm;
            var maxId = acquirerInputModel.MaxId;
            var sinceId = acquirerInputModel.SinceId;
            var batchSize = acquirerInputModel.BatchSize;

            while (true)
            {
                var batch = new List<Status>();
                try
                {
                    _logger.LogInformation("Downloading nest posts. Earliest id: {id}", acquirerInputModel.MaxId);

                    var search = await GetStatusBatchAsync(searchTerm, batchSize, maxId: maxId, sinceId: sinceId);

                    batch = search
                        .Statuses
                        .ToList();

                    _logger.LogInformation("Found {count} posts. Time {time}",
                        batch.Count,
                        search.Statuses.FirstOrDefault()?.CreatedAt.ToString("s"));

                    maxId = search.Statuses.Any()
                        ? Math.Min(search.Statuses.Min(status => status.StatusID) - 1, acquirerInputModel.MaxId)
                        : acquirerInputModel.MaxId;
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while getting data: {error}, type {type}", e.Message, e.GetType().Name);
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
            string language = "cs",
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
                _logger.LogError("Error during context initialization", e.Message);
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
