using Domain.Acquisition;
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


    public class TwitterDataAcqirer : IDataAcquirer
    {
        private TwitterContext _twitterContext;
        private readonly ILogger<TwitterDataAcqirer> _logger;

        public TwitterDataAcqirer(
            ILogger<TwitterDataAcqirer> logger)
        {
            _logger = logger;
        }

        public async Task<DataAcquirerOutputModel> AcquireBatchAsync(
            DataAcquirerInputModel acquirerInputModel,
            CancellationToken cancellationToken)
        {
            // TODO refactor using some session establisher. this is fucked up
            if (_twitterContext == null)
            {
                CreateContext(acquirerInputModel);
            }

            try
            {
                var search = await GetStatusBatch(acquirerInputModel);

                var searchResponse = search
                    .Statuses
                    .Select(r => FromStatus(acquirerInputModel, r))
                    .ToList();

                return new DataAcquirerOutputModel()
                {
                    Posts = searchResponse,
                    MaxId = search.Statuses.Min(status => status.StatusID) - 1
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<Search> GetStatusBatch(DataAcquirerInputModel acquirerInputModel)
        {
            string searchTerm = acquirerInputModel.Query;

            var sinceId = acquirerInputModel.FromId.HasValue ? acquirerInputModel.FromId : 1;

            var combinedSearchResults = new List<Status>();


            return await _twitterContext.Search
                .Where(search => search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.Count == acquirerInputModel.NumberOfPostToRetrieve &&
                       search.SinceID == sinceId &&
                       search.TweetMode == TweetMode.Extended)
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
