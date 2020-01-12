using System.Threading.Tasks;

namespace Infrastructure.Twitter.Abstract
{
    public interface ITwitterContextProvider
    {
        Task<ITwitterContext> GetContextAsync(
            TwitterCredentials credentials);
    }
}
