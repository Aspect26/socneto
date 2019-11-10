using System;
using System.Collections.Generic;
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
        private readonly IEnumerator<UniPostStaticData> _postsEnumerator;
        private IStaticDataProvider _dataProvider;

        public StaticDataEnumerator(
            IStaticDataProvider dataProvider,
            IOptions<StaticDataOptions> randomGenratorOptionsAccessor)
        {
            _dataProvider = dataProvider;
            _downloadSimulatedDelay = randomGenratorOptionsAccessor.Value.DownloadDelay;
            _postsEnumerator = dataProvider.GetEnumerator();
        }

        public async IAsyncEnumerable<UniPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel)
        {
            var count = acquirerInputModel.BatchSize;

            ulong id = 0;
            var posts = new List<UniPost>();
            for (int i = 0; i < count; i++)
            {
                if (!_postsEnumerator.MoveNext())
                {
                    _postsEnumerator.Reset();
                    _postsEnumerator.MoveNext();
                }

                var post = _postsEnumerator.Current;
                var uniPost = UniPost.FromValues(
                    post.PostId,
                    post.Text,
                    post.Source,
                    post.UserId,
                    post.PostDateTime,
                    acquirerInputModel.JobId);

                posts.Add(uniPost);
            }

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
