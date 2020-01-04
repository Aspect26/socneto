using Domain;
using Domain.Acquisition;
using Domain.Model;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Twitter
{
    public class TwitterDataAcquirer : IDataAcquirer
    {
        private readonly TwitterBatchLoaderFactory _twitterBatchLoaderFactory;
        private readonly TwitterContextProvider _twitterContextProvider;
        private readonly IEventTracker<TwitterDataAcquirer> _logger;

        public TwitterDataAcquirer(
            TwitterBatchLoaderFactory twitterBatchLoaderFactory,
            TwitterContextProvider twitterContextProvider,
            IEventTracker<TwitterDataAcquirer> logger)
        {
            _twitterBatchLoaderFactory = twitterBatchLoaderFactory;
            _twitterContextProvider = twitterContextProvider;
            _logger = logger;
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel)
        {
            var credentials = ExtractCredentials(acquirerInputModel);
            var twitterContext = await _twitterContextProvider.GetContextAsync(credentials);

            var subqueries = ParseTwitterQuery(acquirerInputModel.Query);

            var asyncEnumerators = subqueries.Select(query =>
            {
                var batchLoader = _twitterBatchLoaderFactory.Create(acquirerInputModel.JobId);
                var defaultMetadata = CreateDefaultMetadata(query, acquirerInputModel);
                return batchLoader.CreateBatchPostEnumerator(twitterContext, defaultMetadata);
            })                ;

            var it = AsyncEnumeratorConfluctor.AggregateEnumerables(asyncEnumerators);
            await foreach (var item in it)
            {
                yield return item;
            }

            //var query = subqueries.First();

            //var defaultMetadata = CreateDefaultMetadata(query,acquirerInputModel, credentials);
            //var metadata = await context.GetOrCreateAsync(defaultMetadata);


            //long foundPosts = 0;
            //var mostRecentPost = DateTime.MinValue;

            //while (true)
            //{
            //    _logger.TrackInfo("Now searching for {query}", query);
            //    var twitterInputModel = new TwitterQueryInput(
            //        query,
            //        metadata.Language,
            //        metadata.MaxId,
            //        metadata.SinceId,
            //        metadata.BatchSize);

            //    var batches = QueryPastPostAsync(
            //        twitterContext,
            //        acquirerInputModel.JobId,
            //        twitterInputModel);

            //    await foreach (var batch in batches)
            //    {
            //        if (!batch.Any())
            //        {
            //            _logger.TrackWarning(
            //                "QueryData",
            //                "Querying yielded empty batch",
            //                new
            //                {
            //                    query = metadata.Query,
            //                    jobId = acquirerInputModel.JobId
            //                });
            //            break;
            //        }


            //        foreach (var post in batch)
            //        {
            //            metadata.MaxId = Math.Min(post.StatusID - 1, metadata.MaxId);
            //            metadata.SinceId = Math.Max(post.StatusID, metadata.SinceId);

            //            if (post.CreatedAt > mostRecentPost)
            //            {
            //                mostRecentPost = post.CreatedAt;
            //            }
            //            yield return FromStatus(post);
            //        }
            //        foundPosts += batch.Count;

            //        _logger.TrackStatistics("QueryDataStatistics",
            //            new
            //            {
            //                foundPosts,
            //                mostRecentPost,
            //                query = metadata.Query,
            //                language = metadata.Language,
            //                jobId = acquirerInputModel.JobId
            //            });

            //        await context.UpdateAsync(metadata);
            //    }

            //    metadata.MaxId = ulong.MaxValue;
            //    await context.UpdateAsync(metadata);

            //    await Task.Delay(TimeSpan.FromMinutes(.01));
            //}
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
#warning hardcodedstuff here
                MaxId = 1213343455193137152,

                SinceId = 0,
                Language = acquirerInputModel.QueryLanguage,
                Query = query,
                BatchSize = acquirerInputModel.BatchSize
            };
        }

    }
}
