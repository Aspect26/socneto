using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.EventTracking;
using Domain.Model;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace Infrastructure.Reddit
{

    public class RedditDataAcquirer : IDataAcquirer
    {
        private readonly RedditContextProvider _redditContextProvider;
        private readonly IEventTracker<RedditDataAcquirer> _eventTracker;
        private readonly StringComparer _stringComparer
            = StringComparer.Create(CultureInfo.InvariantCulture, true);
        public RedditDataAcquirer(
            RedditContextProvider redditContextProvider,
            IEventTracker<RedditDataAcquirer> eventTracker)
        {
            _redditContextProvider = redditContextProvider;
            _eventTracker = eventTracker;
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel,
            [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var credentials = ExtractCredentials(acquirerInputModel);
            var reddit = await _redditContextProvider.GetContextAsync(credentials);

            var query = acquirerInputModel.Query;

            var limit = 50;
            DateTime? before = null;

            while (true)
            {
                var maxBefore = before;
                var count = 0;
                string after = null;
                var postListing = GetPosts(reddit, after, limit, query, count);
                var outDated = false;
                while (postListing.Count > 0)
                {
                    var children = postListing;
                    foreach (var item in children)
                    {
                        if (item.Created <= before)
                        {
                            outDated = true;
                            break;
                        }
                        count++;
                        maxBefore = Max(item.Created, maxBefore);

                        yield return FromPost(item, query);
                        var comments = item.Comments.GetTop(100);
                        foreach (var c in comments)
                        {
                            var listingPost = item.Listing;
                            yield return DataAcquirerPost.FromValues(
                                listingPost.Id,
                                //"(title:" + listingPost.Title + ",comment)" + c.Body,
                                c.Body,
                                "en",
                                "reddit",
                                c.Author ?? "n/a",
                                listingPost.CreatedUTC.ToString("s"),
                                query);
                        }
                    }

                    if (outDated)
                    {
                        break;
                    }
                    after = postListing.Count > 0 ? postListing.Last().Fullname : after;

                    postListing = GetPosts(reddit, after, limit, query, count);
                }
                before = maxBefore;

                await Task.Delay(TimeSpan.FromMinutes(10));
            }
        }

        private List<Post> GetPosts(
               RedditClient reddit,
               string after,
               int limit,
               string query,
               int count)
        {
            var searchInput = new SearchGetSearchInput(
                                    q: query,
                                    after: after,
                                    limit: limit,
                                    count: count);
            return reddit.Search(searchInput);
        }

        private string Max(string a, string b)
        {
            var cmpResult = _stringComparer.Compare(a, b);
            if (cmpResult > 0)
            {
                return a;
            }
            return b;
        }



        private DataAcquirerPost FromPost(Post r, string query)
        {
            var listingPost = r.Listing;
            return DataAcquirerPost.FromValues(
                listingPost.Id,
                //"(title:" + listingPost.Title + ")" + listingPost.SelfText,
                listingPost.SelfText,
                "en",
                "reddit",
                r.Author ?? "n/a",
                listingPost.CreatedUTC.ToString("s"),
                query);
        }

        private RedditCredentials ExtractCredentials(DataAcquirerInputModel acquirerInputModel)
        {
            return new RedditCredentials(
                acquirerInputModel.Attributes.GetValue("appId"),
                acquirerInputModel.Attributes.GetValue("appSecret"),
                acquirerInputModel.Attributes.GetValue("refreshToken"));
        }
        private DateTime Max(DateTime a, DateTime? b)
        {
            if (!b.HasValue)
            {
                return a;
            }

            if (a < b.Value)
            {
                return b.Value;
            }
            return a;
        }
    }

}
