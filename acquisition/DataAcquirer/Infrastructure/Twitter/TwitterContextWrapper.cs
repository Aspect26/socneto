using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Twitter.Abstract;
using LinqToTwitter;

namespace Infrastructure.Twitter
{
    public class TwitterContextWrapper : ITwitterContext
    {
        private readonly TwitterContext _context;

        public TwitterContextWrapper(TwitterContext context)
        {
            _context = context;
        }

        public async Task<Search> GetStatusBatchAsync(
            string searchTerm,
            int batchSize,
            string language,
            ulong maxId = ulong.MaxValue,
            ulong sinceId = 1)
        {
            var combinedSearchResults = new List<Status>();
            if(maxId <sinceId)
            {
                return new Search()
                {
                    Statuses = new List<Status>()
                };
            }
            // HOTFIX Null language results in exception saying that 
            // "Authorization did not succeeded". 
            if (string.IsNullOrEmpty(language))
            {
                return await _context.Search
                    .Where(search => search.Type == SearchType.Search &&
                           search.Query == searchTerm &&
                           search.Count == batchSize &&
                           search.MaxID == maxId &&
                           search.SinceID == sinceId &&
                           search.TweetMode == TweetMode.Extended
                           )
                    .SingleOrDefaultAsync();
            }
            else
            {
                return await _context.Search
                 .Where(search => search.Type == SearchType.Search &&
                        search.Query == searchTerm &&
                        search.SearchLanguage == language &&
                        search.Count == batchSize &&
                        search.TweetMode == TweetMode.Extended &&
                        search.MaxID == maxId &&
                        search.SinceID == sinceId)
                 .SingleOrDefaultAsync();
            }
        }

    }
}
