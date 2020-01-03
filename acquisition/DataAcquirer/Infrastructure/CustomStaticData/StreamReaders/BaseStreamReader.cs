using System.Collections.Concurrent;
using Domain.Model;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class BaseStreamReader
    {
        protected readonly BlockingCollection<DataAcquirerPost>
           _posts = new BlockingCollection<DataAcquirerPost>(1000);

        public bool IsCompleted => _posts.IsCompleted;

        public bool TryGetPost(out DataAcquirerPost post)
        {
            if (IsCompleted)
            {
                post = null;
                return false;
            }
            return _posts.TryTake(out post);
        }

        protected PostBuilder BuildMutable()
        {
            return new PostBuilder();
        }
    }
}
