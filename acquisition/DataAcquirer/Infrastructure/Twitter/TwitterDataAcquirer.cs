using Domain;
using Domain.Acquisition;
using Domain.EventTracking;
using Domain.Model;
using Infrastructure.Twitter.Abstract;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Infrastructure.Twitter
{
    public class TwitterDataAcquirer : IDataAcquirer
    {
        private readonly TwitterBatchLoaderFactory _twitterBatchLoaderFactory;
        private readonly ITwitterContextProvider _twitterContextProvider;
        private readonly IEventTracker<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            TwitterBatchLoaderFactory twitterBatchLoaderFactory,
            ITwitterContextProvider twitterContextProvider,
            IEventTracker<TwitterDataAcquirer> logger)
        {
            _twitterBatchLoaderFactory = twitterBatchLoaderFactory;
            _twitterContextProvider = twitterContextProvider;
            _logger = logger;
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel,
            [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {

            var credentials = ExtractCredentials(acquirerInputModel);
            var twitterContext = await _twitterContextProvider.GetContextAsync(credentials);

            var subqueries = ParseTwitterQuery(acquirerInputModel.Query);

            var asyncEnumerators = subqueries.Select(query =>
            {
                var batchLoader = _twitterBatchLoaderFactory.Create(acquirerInputModel.JobId);
                var defaultMetadata = CreateDefaultMetadata(query, acquirerInputModel);
                return batchLoader.CreateBatchPostEnumerator(twitterContext, defaultMetadata);
            });

            var it = AsyncEnumeratorConfluctor.AggregateEnumerables(
                asyncEnumerators,
                cancellationToken);
            await foreach (var item in it)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                yield return item;
            }
        }

        private List<string> ParseTwitterQuery(string query)
        {
            var subqueries = query.Split(';', StringSplitOptions.RemoveEmptyEntries);

            return subqueries.Select(r => r
                .Replace("NOT ", "-")
                .Replace(" AND ", " "))
                .ToList();
        }

        private static TwitterCredentials ExtractCredentials(DataAcquirerInputModel acquirerInputModel)
        {
            // TODO validate credentials
            return new TwitterCredentials
            {
                ConsumerKey = acquirerInputModel.Attributes.GetValue("ApiKey"),
                ConsumerSecret = acquirerInputModel.Attributes.GetValue("ApiSecretKey"),
                AccessToken = acquirerInputModel.Attributes.GetValue("AccessToken"),
                AccessTokenSecret = acquirerInputModel.Attributes.GetValue("AccessTokenSecret")
            };
        }

        private static TwitterMetadata CreateDefaultMetadata(
           string query,
           DataAcquirerInputModel acquirerInputModel)
        {
            return new TwitterMetadata
            {
                MaxId = ulong.MaxValue,
                SinceId = 0,
                Language = acquirerInputModel.QueryLanguage,
                Query = query,
                BatchSize = acquirerInputModel.BatchSize
            };
        }

    }
}
