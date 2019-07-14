using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Acquisition;
using Domain.Model;

namespace Infrastructure.DataGenerator
{
    public class DataGeneratorAcquirer :IDataAcquirer
    {
        private Random _random = new Random(123);
        
        public async  Task<DataAcquirerOutputModel> AcquireBatchAsync(DataAcquirerInputModel acquirerInputModel, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            var uniPosts = Enumerable
                .Range(0, 100)
                .Select(r => _random.Next())
                .Select(GetRandomPost)
                .ToList();

            return new DataAcquirerOutputModel
            {
                Posts = uniPosts
            };

        }

        private UniPost GetRandomPost(int seed)
        {
            var postText = GetRandomString(seed, 100);
            var postSource = "random-data";
            var postUser = GetRandomString(seed,12);
            var dateTimeString = DateTime.Now.ToString("s");

            return UniPost.FromValues(
                postText,
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

        public static string GetRandomString(int seed,int length)
        {
            var random = new Random(seed);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
