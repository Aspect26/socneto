using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Options;
using Socneto.Domain;

namespace Socneto.Coordinator.Infrastructure
{
    public class TwitterOptions
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
    public class TwitterApi //:ISocialNetwork
    {
        const int MaxSearchEntriesToReturn = 100;
        private readonly TwitterContext _twitterContext;

        
        public TwitterApi(IOptions<TwitterOptions> twitterOptions)
        {
            _twitterContext = ConstructTwitterContext(
                twitterOptions.Value.ConsumerKey
                ,twitterOptions.Value.ConsumerSecret).Result;
        }


        public async Task<IList<UniPost>> SearchAsync(string searchTerm)
        {
            return await OneTimeSearchAsync(searchTerm);
            //return await DoPagedSearchAsync(searchTerm);
        }
     
        private async Task<IList<UniPost>> DoPagedSearchAsync(string searchTerm)   
        {
            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID;

            var combinedSearchResults = new List<UniPost>();

            var searchResponse =await GetBatch(searchTerm, sinceID);

            combinedSearchResults.AddRange(searchResponse.Items);
            ulong previousMaxID = ulong.MaxValue;
            do
            {
                // one less than the newest id you've just queried
                maxID = searchResponse.MaxId - 1;
                searchResponse =await GetBatch(searchTerm, sinceID, maxID);

                combinedSearchResults.AddRange(searchResponse.Items);

            } while (searchResponse.HasAny());

            return combinedSearchResults;
            //combinedSearchResults.ForEach(tweet =>
            //    Console.WriteLine(
            //        "\n  User: {0} ({1})\n  Tweet: {2}",
            //        tweet.User.ScreenNameResponse,
            //        tweet.User.UserIDResponse,
            //        tweet.Text));
        }
        private async Task<IList<UniPost>> OneTimeSearchAsync(string searchTerm)
        {
            //string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter";
            //string searchTerm = "#ömer -RT -instagram news source%3Afoursquare";

            var searchResponse =
                await _twitterContext.Search
                    .Where(search =>
                        search.Type == SearchType.Search && search.Query == searchTerm)
                    .SingleOrDefaultAsync();

            //if (searchResponse != null && searchResponse.Statuses != null)
            //    return searchResponse.Statuses.Select(r => UniPost.FromValues(r.Text,"todo")).ToList();

            throw  new NotImplementedException();
            return new List<UniPost>();

            //searchResponse.Statuses.ForEach(tweet =>
            //    Console.WriteLine(
            //        "\n  User: {0} ({1})\n  Tweet: {2}",
            //        tweet.User.ScreenNameResponse,
            //        tweet.User.UserIDResponse,
            //        tweet.Text));
        }

        private async Task<PagedSearchResult<UniPost>> GetBatch(string searchTerm, ulong sinceId,ulong? maxId = null)
        {

            var searchQuery = _twitterContext.Search;
            IQueryable<Search> queryable = null;
            if (maxId.HasValue)
                queryable = searchQuery.Where(search =>
                    search.Type == SearchType.Search &&
                    search.Query == searchTerm &&
                    search.Count == MaxSearchEntriesToReturn &&
                    search.MaxID == maxId.Value &&
                    search.SinceID == sinceId);
            else
                queryable = searchQuery.Where(search =>
                    search.Type == SearchType.Search &&
                    search.Query == searchTerm &&
                    search.Count == MaxSearchEntriesToReturn &&
                    search.SinceID == sinceId);


            var items =await queryable.Select(r => r.Statuses).SingleOrDefaultAsync();
            var newMaxId = items.Select(r => r.MaxID).Min();
            
            //var uniPost = items .Select(s => s.ToUnifiedPost());
            throw new NotImplementedException();
            //return new PagedSearchResult<UniPost>(uniPost, newMaxId);
        }

        private async Task<TwitterContext> ConstructTwitterContext(string consumerKey,string consumerSecret)
        {

            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret
                },
            };

            await auth.AuthorizeAsync();

            return new TwitterContext(auth);

        }


        private class PagedSearchResult<T>
        {
            public IEnumerable<T> Items { get; }
            public ulong MaxId { get; }

            public PagedSearchResult(IEnumerable<T> items, ulong maxId)
            {
                Items = items;
                MaxId = maxId;
            }

            public bool HasAny()
            {
                return Items.Any();
            }
        }
    }
}
