using System.Threading.Tasks;

namespace Domain.DependencyWaiting
{
    public class AlwaysReadyStorageDependencyWaitingService : IStorageDependencyWaitingService
    {
        public Task<bool> IsDependencyReadyAsync()
        {
            return Task.FromResult(true);
        }
    }
}
