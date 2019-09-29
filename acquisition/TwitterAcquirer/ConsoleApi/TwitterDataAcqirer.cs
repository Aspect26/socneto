using Domain.Acquisition;
using Domain.Model;
using LinqToTwitter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi
{
    public class TwitterDataAcqirer //: IDataAcquirer
    {
        //private readonly TwitterContext _twitterContext;

        //public TwitterDataAcqirer(TwitterContext twitterContext)
        //{
        //    _twitterContext = twitterContext;
        //}

        //public async Task<DataAcquirerOutputModel> AcquireBatchAsync(DataAcquirerInputModel acquirerInputModel, CancellationToken cancellationToken)
        //{
        //    var searchResponse =
        //        await
        //        (from search in _twitterContext.Search
        //         where search.Type == SearchType.Search &&
        //               search.Query == "\"LINQ to Twitter\""
        //         select search)
        //        .SingleOrDefaultAsync();

        //    if (searchResponse != null && searchResponse.Statuses != null)
        //        searchResponse.Statuses.ForEach(tweet =>
        //            Console.WriteLine(
        //                "User: {0}, Tweet: {1}",
        //                tweet.User.ScreenNameResponse,
        //                tweet.Text));
        //}
    }
}
