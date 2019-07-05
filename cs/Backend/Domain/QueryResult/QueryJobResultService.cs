using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.QueryResult
{
    public class QueryJobResultService : IQueryJobResultService
    {
        
        public Task<JobStatus> GetJobStatus(Guid jobId)
        {
            return Task.FromResult( RandomDataGenerator.GetRandomJobStatusResponse(jobId));
        }

        public Task<JobResult> GetJobResult(Guid jobId)
        {
            var hc = Math.Abs(jobId.GetHashCode());
            var topics = new[] { "Guns", "Cars", "Friends", "Cartoon", "Sunshine" };

            var posts = Enumerable.Range(0, hc % 100)
                .Select(r =>
                {
                    var rand = new Random(r + hc).Next(int.MaxValue);
                    return new Post
                    {
                        Keywords = new List<string>() { topics[rand % topics.Length] },
                        Sentiment = (double)(rand) / int.MaxValue,
                        Text = RandomDataGenerator.RandomString(64 + rand % 64),
                        UserId = hc,
                        DateTime = RandomDataGenerator.GetRandomDate(r + hc)
                    };
                })
                .ToList();


            var jobResultResponse = new JobResult
            {
                InputQuery = topics[hc % topics.Length],
                Posts = posts,
                JobId = jobId
            };

            return Task.FromResult(jobResultResponse);
        }
    }
}
