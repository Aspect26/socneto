using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services.Mock
{
    public class MockStorageService : IStorageService
    {
        
        private readonly List<User> _users = new List<User>
        { 
            new User { Username = "admin", Password = "admin" } 
        };
        
        public async Task<User> GetUser(string username)
        {
            return await Task.Run(() => _users.SingleOrDefault(x => x.Username == username));
        }

        public async Task<IList<JobStatus>> GetUserJobs(string username)
        {
            var random = new Random(username.GetHashCode());

            var jobStatuses =  (IList<JobStatus>)Enumerable.Range(0, random.Next(5, 15))
                .Select(r =>
                {
                    var arr = new byte[16];
                    random.NextBytes(arr);
                    return new Guid(arr);
                })
                .Select(RandomDataGenerator.GetRandomJobStatusResponse)
                .ToList();

            return await Task.FromResult(jobStatuses);
        }

        public async Task<JobStatus> GetJob(Guid jobId)
        {
            return await Task.FromResult(RandomDataGenerator.GetRandomJobStatusResponse(jobId));
        }

        public async Task<IList<AnalyzedPost>> GetAnalyzedPosts(Guid jobId)
        {
            var hc = Math.Abs(jobId.GetHashCode());
            var posts = Enumerable.Range(0, hc % 100)
                .Select(r =>
                {
                    var random = new Random(r + hc);
                    var rand = random.Next(int.MaxValue);
                    var post = new Post
                    {
                        AuthorId = RandomDataGenerator.RandomString(10),
                        Text = RandomDataGenerator.RandomString(64 + rand % 64),
                        PostedAt = RandomDataGenerator.GetRandomDate(r + hc)
                    };

                    return new AnalyzedPost
                    {
                        JobId = jobId,
                        Post = post,
                        Analysis = new Dictionary<string, Dictionary<string, AnalysisValue>> ()
                        {
                            { "sentiment", new Dictionary<string, AnalysisValue>
                                {
                                    { "polarity", new AnalysisValue
                                        {
                                            ValueType = AnalysisValueType.Number,
                                            Value = random.NextDouble() > 0.5? 1 : -1
                                        }
                                    },
                                    {
                                        "accuracy", new AnalysisValue
                                        {
                                            ValueType = AnalysisValueType.Number,
                                            Value = random.NextDouble()
                                        }
                                    }
                                }
                            }
                        }
                    };
                })
                .ToList();
            

            return await Task.FromResult(posts);
        }
    }
}
