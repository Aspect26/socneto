using System.Threading.Tasks;

namespace Domain.DependencyWaiting
{
    public interface IStorageDependencyWaitingService
    {
        Task<bool> IsDependencyReadyAsync();
    }

    public interface IKafkaDependencyWaitingService
    {
        Task<bool> IsDependencyReadyAsync();
    }
}
