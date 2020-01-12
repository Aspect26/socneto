using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Acquisition;
using Domain.JobManagement;
using Domain.Model;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataGenerator
{

    public class RandomDataGeneratorAcquirer : IDataAcquirer
    {
        private readonly Random _random;
        private readonly TimeSpan _downloadDelay;

        public RandomDataGeneratorAcquirer(
            IOptions<RandomGeneratorOptions> randomGenratorOptionsAccessor
            )
        {
            _downloadDelay = randomGenratorOptionsAccessor.Value.DownloadDelay;
            _random = new Random(randomGenratorOptionsAccessor.Value.Seed);
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel jobConfig,
            [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await Task.Delay(_downloadDelay);

            var uniPosts = Enumerable
                .Range(0, 100)
                .Select(r => _random.Next())
                .Select(GetRandomPost);

            foreach (var post in uniPosts)
            {
                yield return post;
            }
        }

        private DataAcquirerPost GetRandomPost(int seed)
        {
            var postText = GetRandomString(seed, 100);
            var postSource = "random-data";
            var postUser = GetRandomString(seed, 12);
            var dateTimeString = DateTime.Now.ToString("s");

            var id = Guid.NewGuid();
            return DataAcquirerPost.FromValues(
                $"tw-{id}",
                postText,
                "en",
                postSource,
                postUser,
                dateTimeString);
        }

        private static DateTime GetRandomDate(int seed)
        {
            var year = 2010 + (seed % 10);
            var month = (seed % 12) + 1;
            var day = (seed % 28) + 1;
            var hour = seed % 24;
            var minute = ((seed - 53) % 59) + 1;
            var second = (seed % 59) + 1;
            //Console.WriteLine($"{year} {month} {day} {hour} {minute} {second}");
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static string GetRandomString(int seed, int length)
        {
            var random = new Random(seed);
            const string chars = "   ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
