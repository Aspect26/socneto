using System.Threading.Tasks;

namespace Domain.DependencyWaiting
{
    public class AlwaysReadyKafkaDependency : IKafkaDependencyWaitingService
    {
        public Task<bool> IsDependencyReadyAsync()
        {
            return Task.FromResult(true); 
        }
    }
}
