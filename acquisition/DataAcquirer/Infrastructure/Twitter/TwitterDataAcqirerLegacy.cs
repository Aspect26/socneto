using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Model;
using System.Collections.Generic;
using System.Threading;

namespace Infrastructure.Twitter
{

  

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
