using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Socneto.Domain.Models.Storage.Response;
using Socneto.Domain.Services;

namespace Tests
{
    [TestClass]
    public class JobServiceTest
    {
        [TestMethod]
        public async Task NoJobDetailsTest()
        {
            const string mockUserName = "Bilbo";
            IList<Job> emptyJobList = new List<Job>();
            
            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetUserJobs(mockUserName)).Returns(Task.FromResult(emptyJobList));

            var jobService = new JobService(mockStorageService.Object);
            var result = await jobService.GetJobsDetails(mockUserName);

            Assert.AreEqual(emptyJobList, result, "When storage returns no job, backend should also return empty list");
        }
        
        [TestMethod]
        public async Task SomeJobDetailsTest()
        {
            const string mockUserName = "Smaug";
            IList<Job> someJobsList = new List<Job>()
            {
                new Job{JobId = Guid.NewGuid(), Username = mockUserName, JobName = "Protect gold"},
                new Job{JobId = Guid.NewGuid(), Username = mockUserName, JobName = "Protect Arkenstone"},
                new Job{JobId = Guid.NewGuid(), Username = mockUserName, JobName = "Burn lake-town"},
            };
            
            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetUserJobs(mockUserName)).Returns(Task.FromResult(someJobsList));

            var jobService = new JobService(mockStorageService.Object);
            var result = await jobService.GetJobsDetails(mockUserName);

            Assert.AreEqual(someJobsList, result, "The backend should return jobs that it received from storage");
        }

        [TestMethod]
        public async Task GetSomePostsTest()
        {
            var mockJobId = Guid.NewGuid();
            var someDate = DateTime.Now;
            var mockPage = 1;
            var mockPageSize = 50;
            var mockPosts = new List<Post>
            {
                new Post{Text = "What has roots as nobody sees, is taller than trees, up, up it goes, and yet never grows?"},
                new Post{Text = "Voiceless it cries, wingless flutters, toothless bites, mouthless mutters."},
                new Post{Text = "It cannot be seen, cannot be felt, cannot be heard, cannot be smelt. it lies behind stars and under hills, and empty holes it fills. it comes out first and follows after, ends life, kills laughter."}
            };
            var mockPostsCount = 6;
            
            var mockStorageService = new Mock<IStorageService>();
            var listWithCount = new ListWithCount<Post> {Data = mockPosts, TotalCount = mockPostsCount};
            
            mockStorageService.Setup(o => o.GetPosts(mockJobId, new string[0], new string[0], someDate, someDate, mockPage - 1, mockPageSize)).Returns(Task.FromResult(listWithCount));
            
            var jobService = new JobService(mockStorageService.Object);
            var result = await jobService.GetJobPosts(mockJobId, new string[0], new string[0], someDate, someDate, mockPage, mockPageSize);
            var expectedResult = (mockPosts, 3);
            
            Assert.AreEqual(expectedResult.Item1, result.Item1, "The backend should return posts that it received from storage");
            Assert.AreEqual(mockPostsCount, result.Item2, "The backend should return total number of posts that it received from storage");
        }
        
    }
}
