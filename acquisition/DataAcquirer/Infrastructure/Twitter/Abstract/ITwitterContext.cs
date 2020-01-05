using System.Threading.Tasks;
using LinqToTwitter;

namespace Infrastructure.Twitter.Abstract
{
    public interface ITwitterContext
    {
        Task<Search> GetStatusBatchAsync(
            string searchTerm,
            int batchSize,
            string language,
            ulong maxId = ulong.MaxValue,
            ulong sinceId = 1);
    }
}
