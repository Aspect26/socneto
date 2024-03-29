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
using Infrastructure.StaticData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataGenerator
{

    public class StaticDataEnumerator : IDataAcquirer
    {
        private readonly TimeSpan _downloadSimulatedDelay;
        private readonly IEnumerable<UniPostStaticData> _postsEnumerator;
        private readonly IStaticDataProvider _dataProvider;

        public StaticDataEnumerator(
            IStaticDataProvider dataProvider,
            IOptions<StaticDataOptions> randomGenratorOptionsAccessor)
        {
            _dataProvider = dataProvider;
            _downloadSimulatedDelay = randomGenratorOptionsAccessor.Value.DownloadDelay;
            _postsEnumerator = _dataProvider.GetEnumerable();
        }

        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel,
            [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            ulong id = 0;
            while (true)
            {
                var count = acquirerInputModel.BatchSize;

                var posts = _postsEnumerator
                    .Take(count)
                    .Select(post =>
                     DataAcquirerPost.FromValues(
                        post.OriginalPostId,
                        post.Text,
                        post.Language,
                        post.Source,
                        post.UserId,
                        post.PostDateTime))
                    .ToList();


                id += (ulong) count;
                try
                {
                    await Task.Delay(_downloadSimulatedDelay, CancellationToken.None);
                }
                catch (TaskCanceledException)
                {
                }

                foreach (var post in posts)
                {
                    yield return post;
                };
            }
        }
    }

}
