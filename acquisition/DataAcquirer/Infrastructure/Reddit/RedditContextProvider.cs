using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Reddit;
using Reddit.Inputs.Search;

namespace Infrastructure.Reddit
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

                //var searchInput = new SearchGetSearchInput(q: "foo bar", count: 5);
                //var s = redditContext.Search(searchInput);

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

}
