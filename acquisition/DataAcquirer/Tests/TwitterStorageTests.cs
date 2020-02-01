using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Acquisition;
using Domain.EventTracking;
using Domain.Model;
using Infrastructure.Twitter;
using Infrastructure.Twitter.Abstract;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Unit.TestClasses;

namespace Tests.Unit
{
    [TestClass]
    public class TwitterStorageTests
    {
        [TestMethod]
        public async Task TestStorageAsync()
        {
            var limit = (ulong) 301;
            var maxTweetId = (ulong) 300;
            var testTwitterContext = new TestTwitterContext(limit, maxTweetId);
            var starting = (ulong) 100;
            var batchSize = 3;
            var query = "foo";
            string language = null;
            var jobId = Guid.Parse("d2474631-3d4c-4b07-a992-9d9c1f269cd4");

            var metadata = new TwitterMetadata
            {
                BatchSize = 3,
                MaxId = starting,
                SinceId = 0,
                Query = "foo",
                Language = null
            };
            var metadataContextMock = new Mock<IDataAcquirerMetadataContext>();
            metadataContextMock
                .Setup(r => r.GetOrCreateAsync(It.IsAny<TwitterMetadata>()))
                .Returns(Task.FromResult(metadata));

            var metaDataContextProviderMock = new Mock<IDataAcquirerMetadataContextProvider>();
            metaDataContextProviderMock.Setup(r => r.Get(It.IsAny<Guid>()))
                .Returns(metadataContextMock.Object);

            var batchLoaderOptions = Options.Create(new TwitterBatchLoaderOptions
            {
                NoPostWaitDelay = TimeSpan.Zero,
                ErrorEncounteredWaitDelay = TimeSpan.Zero,
                RateLimitExceededWaitDelay = TimeSpan.Zero
            });

            var batchLoaderFactory = new TwitterBatchLoaderFactory(
                batchLoaderOptions,
                new Mock<IEventTracker<TwitterBatchLoader>>().Object,
                metaDataContextProviderMock.Object);

            var contextProviderMock = new Mock<ITwitterContextProvider>();
            contextProviderMock
                .Setup(r => r.GetContextAsync(It.IsAny<TwitterCredentials>()))
                .Returns(Task.FromResult<ITwitterContext>(testTwitterContext));

            var trackerMocq = new Mock<IEventTracker<TwitterDataAcquirer>>();
            var twitterDataAcquirer = new TwitterDataAcquirer(
                batchLoaderFactory,
                contextProviderMock.Object,
                trackerMocq.Object);


            var input = new DataAcquirerInputModel(
                jobId,
                query,
                language,
                new DataAcquirerAttributes(new Dictionary<string, string>()),
                batchSize);

            var posts = twitterDataAcquirer.GetPostsAsync(input);

            var ids = new List<ulong>();

            await foreach (var post in posts)
            {
                if ((ulong)ids.Count >= maxTweetId)
                {
                    break;
                }
                var idStr = post.OriginalPostId;
                var id = ulong.Parse(idStr);
                ids.Add(id);
            }

            var actual = ids.OrderBy(r => r).ToList();
            var expected = Enumerable.Range(1, (int) maxTweetId).Select(r => (ulong) r).ToList();

            CollectionAssert.AreEqual(
                expected,
                actual);

        }
    }
}
