using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.Model;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using Reddit.Inputs.Search;

namespace ConsoleApi.Reddit
{

    public class RedditContextProvider
    {
        private readonly ConcurrentDictionary<string, RedditClient> _redditClients =
            new ConcurrentDictionary<string, RedditClient>();
        private readonly IEventTracker<RedditContextProvider> _eventTracker;

        public RedditContextProvider(
            IEventTracker<RedditContextProvider> eventTracker)
        {
            _eventTracker = eventTracker;
        }
        public Task<RedditClient> GetContextAsync(RedditCredentials credentials)
        {
            credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            var props = new[]
            {
                credentials.AppId,
                credentials.ApiSecret,
                credentials.RefreshToken
            };
            if (props.Any(r => string.IsNullOrEmpty(r)))
            {
                throw new ArgumentException("Credentials field contains null value", nameof(credentials));
            }

            var key = string.Join('+', props);
            if (_redditClients.TryGetValue(key, out var context))
            {
                return Task.FromResult(context);
            }

            try
            {
                var redditContext = new RedditClient(
                    appId: credentials.AppId,
                    appSecret: credentials.ApiSecret,
                    refreshToken: credentials.RefreshToken);

                var searchInput = new SearchGetSearchInput(q: "foo bar", count: 5);
                var s = redditContext.Search(searchInput);

                _redditClients.TryAdd(key, redditContext);
                return Task.FromResult(redditContext);

            }
            catch (Exception e)
            {
                _eventTracker.TrackError(
                    "RedditContext",
                    "Encountered error while creating context",
                    new
                    {
                        exception = e
                    });
                throw;
            }
        }

    }

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
            DataAcquirerInputModel acquirerInputModel)
        {
            var credentials = ExtractCredentials(acquirerInputModel);
            var reddit = await _redditContextProvider.GetContextAsync(credentials);

            var query = acquirerInputModel.Query;

#warning Hard coded stuff here
            var before = "t3_ei33zr";
            while (true)
            {
                var after = "";
                var searchPost = GetPostsFromReddit(
                    reddit,
                    query,
                    after,
                    before,
                    acquirerInputModel.BatchSize);
                var maxBefore = before;

                while (searchPost.Count > 0)
                {
                    foreach (var rawPost in searchPost)
                    {
                        maxBefore = Max(rawPost.Fullname, before);

                        var text = rawPost.Listing.SelfText;
                        if (text.Length > 50)
                        {
                            text = text.Substring(0, 50);
                        }
                        var title = rawPost.Title;
                        if (title.Length > 50)
                        {
                            title = title.Substring(0, 50);
                        }
                        Console.WriteLine($"{rawPost.Id}:{rawPost.Listing.CreatedUTC}:{title}:{text}");

                        var post = FromPost(rawPost, query);
                        yield return post;
                    }

                    after = searchPost[searchPost.Count - 1].Fullname;
                    searchPost = GetPostsFromReddit(reddit,
                        query,
                        after,
                        before,
                        acquirerInputModel.BatchSize);
                }
                before = maxBefore;
                await Task.Delay(TimeSpan.FromMinutes(.1));
            }

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

        private static List<Post> GetPostsFromReddit(
            RedditClient reddit,
            string query,
            string after,
            string before,
            int limit)
        {
            //var all = reddit.Subreddit("all");
            //return all.Search(new SearchGetSearchInput(query, after: after));

            var catListingInput = new CategorizedSrListingInput(
                after: after,
                before: before,
                limit: limit);

            return reddit.Subreddit("ApplyingToCollege")
                .Posts.GetNew(catListingInput);

            //var search = new SearchGetSearchInput(query, after: after, before:before, limit:limit);

            //var mixed = reddit.MixedSearch(search);
            ////return mixed.Data.Children.
            //throw new NotImplementedException();
        }

        private DataAcquirerPost FromPost(Post r, string query)
        {
            var listingPost = r.Listing;
            return DataAcquirerPost.FromValues(
                listingPost.Id,
                "(title:" + listingPost.Title + ")" + listingPost.SelfText,
                "en",
                "reddit",
                "n/a",
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
    }

    public class RedditCredentials
    {
        public RedditCredentials(
            string appId,
            string apiSecret,
            string refreshToken)
        {
            AppId = appId;
            ApiSecret = apiSecret;
            RefreshToken = refreshToken;
        }

        public string AppId { get; }
        public string ApiSecret { get; }
        public string RefreshToken { get; }
    }

}
