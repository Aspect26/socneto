using System.Threading.Tasks;
using Domain.Models;

namespace Domain.SubmittedJobConfiguration
{
    public interface IComponentConfigUpdateNotifier
    {
        Task NotifyComponentAsync(string componentConfigChannelName, object notification);
    }
}