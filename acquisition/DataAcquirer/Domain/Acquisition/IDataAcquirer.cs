using System.Collections.Generic;
using System.Text;
using System.Threading;
using Domain.Model;

namespace Domain.Acquisition
{
    public interface IDataAcquirer
    {
        IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
           DataAcquirerInputModel acquirerInputModel,
           CancellationToken cancellationToken = default);
    }
}
