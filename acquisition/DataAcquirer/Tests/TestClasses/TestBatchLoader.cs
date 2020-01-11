using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Infrastructure.Twitter;
using Infrastructure.Twitter.Abstract;
using LinqToTwitter;

namespace Tests.Unit.TestClasses
{

    public class TestTwitterContext : ITwitterContext
    {
        public TestTwitterContext(
            ulong limitCount,
            ulong maxTweetId)
        {
            _limitCount = limitCount;
            _maxTweetId = maxTweetId;
        }

        private readonly ulong _limitCount;
        private readonly ulong _maxTweetId;

        public ulong Count { get; set; }

        public Task<List<Status>> GetStatusBatchAsync(
            string searchTerm,
            int batchSize,
            string language,
            ulong maxId = ulong.MaxValue,
            ulong sinceId = 1)
        {
            if (Count >= _limitCount)
            {
                return Task.FromResult(new List<Status>() { New(ulong.MaxValue) });
            }
            maxId = Math.Min(maxId, _maxTweetId);
            var count = (ulong) batchSize;
            var max = maxId;


            var min = SafeSubtract(max, count);


            var ids = Range(max, min).TakeWhile(r => r > sinceId);

            var statuses = ids
                .Select(r => New(r))
                .ToList();

            return Task.FromResult(statuses);
        }

        private static ulong SafeSubtract(ulong baseNumber, ulong subtractor)
        {
            if (baseNumber < subtractor)
            {
                return 0;
            }
            return baseNumber - subtractor;
        }

        private Status New(ulong id)
        {
            return new Status
            {
                StatusID = id,
                FullText = $"test text {id}",
                Lang = "en",
                UserID = 0,
                CreatedAt = DateTime.Now
            };
        }

        private IEnumerable<ulong> Range(ulong max, ulong min)
        {
            for (var i = max; i > min; i--)
            {
                if (Count >= _limitCount)
                {
                    yield break;
                }
                Count++;
                yield return i;
            }
        }
    }
}
