using Domain;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class TwitterContextProvider
    {
        private readonly ConcurrentDictionary<string, TwitterContext> _contextPerUser
            = new ConcurrentDictionary<string, TwitterContext>();
        private readonly IEventTracker<TwitterContextProvider> _eventTracker;
        private readonly ILogger<TwitterContextProvider> _logger;

        public TwitterContextProvider(
            IEventTracker<TwitterContextProvider> eventTracker,
            ILogger<TwitterContextProvider> logger)
        {
            _eventTracker = eventTracker;
            _logger = logger;
        }

        public async Task<TwitterContext> GetContextAsync(
            TwitterCredentials credentials)
        {
            credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            var props = new[]
            {
                credentials.AccessToken,
                credentials.AccessTokenSecret,
                credentials.ConsumerKey,
                credentials.ConsumerSecret
            };
            if (props.Any(r => string.IsNullOrEmpty(r)))
            {
                throw new ArgumentException("Credentials contain null value", nameof(credentials));
            }

            var key = string.Join('+', props);

            if (_contextPerUser.TryGetValue(key, out var context))
            {
                return context;
            }

            try
            {
                var newContext = CreateContext(credentials);
                await newContext
                    .Search
                    .Where(r => r.Query == "test query"
                    && r.Type == SearchType.Search
                    && r.Count == 1)
                    .SingleOrDefaultAsync();

                _contextPerUser.TryAdd(key, newContext);
                return newContext;

            }
            catch (Exception e)
            {
                _eventTracker.TrackError(
                    "TwitterContext", 
                    "Encountered error while creating context",
                    new
                    {
                        exception = e
                    });
                throw;
            }
        }

        private TwitterContext CreateContext(TwitterCredentials credentials)
        {
            try
            {
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = credentials.ConsumerKey,
                        ConsumerSecret = credentials.ConsumerSecret,
                        AccessToken = credentials.AccessToken,
                        AccessTokenSecret = credentials.AccessTokenSecret
                    }
                };
                return new TwitterContext(auth);
            }
            catch (Exception e)
            {
                _eventTracker.TrackError(
                    "TwitterContext",
                    "Twitter context failed to initialize",
                    new
                    {
                        exception = e
                    });
                throw;
            }
        }
    }
}
