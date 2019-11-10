using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Model;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            var searchTerm = acquirerInputModel.Query;
            var earliestId = ulong.MaxValue;
            var batchSize = acquirerInputModel.BatchSize;

            while (true)
            {
                IList<UniPost> batch = null;
                try
                {
                    _logger.LogInformation("Downloading posts. Earliest id: {id}", acquirerInputModel.EarliestRecordId);

                    var search = await GetStatusBatchAsync(searchTerm,batchSize,maxId:earliestId);

                    batch = search
                        .Statuses
                        .Select(r => FromStatus(acquirerInputModel, r))
                        .ToList();

                    _logger.LogInformation("Found {count} posts. Time {time}",
                        batch.Count,
                        search.Statuses.FirstOrDefault()?.CreatedAt.ToString("s"));

                    earliestId = search.Statuses.Any()
                        ? Math.Min(search.Statuses.Min(status => status.StatusID) - 1, acquirerInputModel.EarliestRecordId)
                        : acquirerInputModel.EarliestRecordId;

                    //var latest = search.Statuses.Any()
                    //    ? Math.Max(search.Statuses.Max(status => status.StatusID) - 1, acquirerInputModel.LatestRecordId)
                    //    : acquirerInputModel.LatestRecordId;

                    //batch = new TwitterDataAcquirerOutputModel(
                    //    searchResponse,
                    //    latest,
                    //    earliest);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while getting data: {error}, type {type}", e.Message, e.GetType().Name);
                }

                foreach (var post in batch)
                {
                    yield return post;
                }

                //// update job metadata

                //// earliestIdPagingParameter = BatchData.EarliestRecordId;
                //latestIdPagingParameter = batchData.LatestRecordId;
                //numberOfPosts += batchData.Posts.Count;

                //var jobMetadata = new
                //{
                //    RecordCount = numberOfPosts,
                //    MaxId = earliestIdPagingParameter,
                //    MinId = batchData.LatestRecordId
                //};

                //if (!batchData.Posts.Any())
                //{
                //    // all old data was already downloaded
                //    // reset parameters so the new ones can be downloaded too

                //    // latest = earliest
                //    // earliest = ulong.MaxValue;
                //}


                //if (!batchData.Posts.Any())
                //{
                //    _logger.LogWarning("No posts were returned, waiting.");
                //    await Task.Delay(TimeSpan.FromMinutes(15));
                //}
                //else
                //{
                //    _logger.LogInformation("So far downloaded {number} of posts", numberOfPosts);
                //}



            }

        }


        private async Task<Search> GetStatusBatchAsync(
            string searchTerm,
            int batchSize,
            string language = "en",
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

    public class TwitterDataAcquirerOutputModel
    {
        public TwitterDataAcquirerOutputModel(
            IList<UniPost> posts,
            ulong minId,
            ulong maxId)
        {
            Posts = posts;
            LatestRecordId = minId;
            EarliestRecordId = maxId;
        }
        public IList<UniPost> Posts { get; }
        public ulong EarliestRecordId { get; }
        public ulong LatestRecordId { get; }

    }

    //public class TwitterDataAcqirerLegacy : IDataAcquirerLegacy
    //{
    //    private TwitterContext _twitterContext;
    //    private readonly ILogger<TwitterDataAcqirerLegacy> _logger;

    //    public TwitterDataAcqirerLegacy(
    //        ILogger<TwitterDataAcqirerLegacy> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public async Task<DataAcquirerOutputModel> AcquireBatchAsync(
    //        DataAcquirerInputModel acquirerInputModel,
    //        CancellationToken cancellationToken)
    //    {
    //        // TODO refactor using some session establisher. this is fucked up
    //        if (_twitterContext == null)
    //        {
    //            // TODO support multiuser
    //            CreateContext(acquirerInputModel);
    //        }

    //        try
    //        {
    //            _logger.LogInformation("Downloading posts. Earliest id: {id}", acquirerInputModel.EarliestRecordId);

    //            var search = await GetStatusBatchAsync(acquirerInputModel);

    //            var searchResponse = search
    //                .Statuses
    //                .Select(r => FromStatus(acquirerInputModel, r))
    //                .ToList();

    //            _logger.LogInformation("Found {count} posts. Time {time}",
    //                searchResponse.Count,
    //                search.Statuses.FirstOrDefault()?.CreatedAt.ToString("s"));

    //            var earliest = search.Statuses.Any()
    //                ? Math.Min(search.Statuses.Min(status => status.StatusID) - 1, acquirerInputModel.EarliestRecordId)
    //                : acquirerInputModel.EarliestRecordId;

    //            var latest = search.Statuses.Any()
    //                ? Math.Max(search.Statuses.Max(status => status.StatusID) - 1, acquirerInputModel.LatestRecordId)
    //                : acquirerInputModel.LatestRecordId;

    //            return new DataAcquirerOutputModel(
    //                searchResponse,
    //                latest,
    //                earliest);
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError("Error while getting data: {error}, type {type}", e.Message, e.GetType().Name);
    //        }
    //        return new DataAcquirerOutputModel(
    //            new List<UniPost>(),
    //            acquirerInputModel.LatestRecordId,
    //            acquirerInputModel.EarliestRecordId);

    //    }

    //    private async Task<Search> GetStatusBatchAsync(DataAcquirerInputModel acquirerInputModel)
    //    {
    //        var searchTerm = acquirerInputModel.Query;

    //        var combinedSearchResults = new List<Status>();

    //        return await _twitterContext.Search
    //            .Where(search => search.Type == SearchType.Search &&
    //                   search.Query == searchTerm &&
    //                   search.SearchLanguage == acquirerInputModel.QueryLanguage &&
    //                   search.Count == acquirerInputModel.BatchSize &&
    //                   search.MaxID == acquirerInputModel.EarliestRecordId &&
    //                   search.SinceID == acquirerInputModel.LatestRecordId &&
    //                   search.TweetMode == TweetMode.Compat)
    //            .SingleOrDefaultAsync();
    //    }

    //    private void CreateContext(DataAcquirerInputModel acquirerInputModel)
    //    {
    //        try
    //        {
    //            var auth = new SingleUserAuthorizer
    //            {
    //                CredentialStore = new SingleUserInMemoryCredentialStore
    //                {
    //                    ConsumerKey = acquirerInputModel.Attributes["ApiKey"],
    //                    ConsumerSecret = acquirerInputModel.Attributes["ApiSecretKey"],
    //                    AccessToken = acquirerInputModel.Attributes["AccessToken"],
    //                    AccessTokenSecret = acquirerInputModel.Attributes["AccessTokenSecret"]
    //                }
    //            };
    //            _twitterContext = new TwitterContext(auth);
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError("Error during context initialization", e.Message);
    //            throw;
    //        }
    //    }

    //    private static UniPost FromStatus(DataAcquirerInputModel acquirerInputModel, Status item)
    //    {
    //        return UniPost.FromValues(
    //                            item.StatusID.ToString(),
    //                            item.Text ?? item.FullText,
    //                            "Twitter",
    //                            item.User.UserID.ToString(),
    //                            item.CreatedAt.ToString("s"),
    //                            acquirerInputModel.JobId);
    //    }
    //}
}
