using Domain.Acquisition;
using Domain.JobManagement;
using Domain.Model;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class TwitterDataAcquirerInputModel
    {
        public TwitterDataAcquirerInputModel(
            string searchTerm,
            ulong maxId,
            ulong sinceId,
            int batchSize)
        {
            SinceId = sinceId;
            BatchSize = batchSize;
            SearchTerm = searchTerm;
            MaxId = maxId;
        }

        public string SearchTerm { get; }
        public ulong MaxId { get; }
        public ulong SinceId { get; }
        public int BatchSize { get; }
    }

    //public class TwitterMetadata
    //{

    //}


    //public interface IDataAcquirerContext
    //{
    //    Task<T> LoadAsync<T>();
    //    Task UpdateAsync<T>(T metadata);
    //}

    public class TwitterDataAcquirer : IDataAcquirer
    {
        private TwitterContext _twitterContext;
        private readonly ILogger<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            ILogger<TwitterDataAcquirer> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<UniPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel)
        {
            // TODO load stored metadata
            
            if (_twitterContext == null)
            {
                // TODO support multiuser
                // TODO use singleton object handling this
                CreateContext(acquirerInputModel);
            }

            var maxId = ulong.MaxValue;

            ulong sinceId = 0;

            while (true)
            {
                var twitterInputModel = new TwitterDataAcquirerInputModel(
                    acquirerInputModel.Query,
                    maxId,
                    sinceId,
                    acquirerInputModel.BatchSize);

                var batches = QueryPastPost(twitterInputModel);

                await foreach (var batch in batches)
                {
                    if(!batch.Any())
                    {
                        _logger.LogWarning("Querying yielded empty batch");

                        break;
                    }
                    foreach (var post in batch)
                    {
                        maxId = Math.Min(post.StatusID - 1, maxId);
                        sinceId = Math.Max(post.StatusID, sinceId);
                        if(maxId <= sinceId)
                        {
                            break;
                        }

                        yield return FromStatus(acquirerInputModel, post);
                    }
                }

                // todo update sinceId, maxId and do it again
                
                maxId = ulong.MaxValue;
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }

        private async IAsyncEnumerable<IList<Status>> QueryPastPost(TwitterDataAcquirerInputModel acquirerInputModel)
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

                if(batch.Count == 0)
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

        private void CreateContext(DataAcquirerInputModel acquirerInputModel)
        {
            try
            {
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = acquirerInputModel.Attributes["ApiKey"],
                        ConsumerSecret = acquirerInputModel.Attributes["ApiSecretKey"],
                        AccessToken = acquirerInputModel.Attributes["AccessToken"],
                        AccessTokenSecret = acquirerInputModel.Attributes["AccessTokenSecret"]
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

        private static UniPost FromStatus(DataAcquirerInputModel acquirerInputModel, Status item)
        {
            return UniPost.FromValues(

                                item.StatusID.ToString(),
                                item.Text ?? item.FullText,
                                "Twitter",
                                item.User.UserID.ToString(),
                                item.CreatedAt.ToString("s"),
                                acquirerInputModel.JobId);
        }
    }
}
