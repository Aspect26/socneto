using System.Threading.Tasks;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Domain.Interfaces
{
    public interface IDataCollector
    {
        Task<DataContainer> CollectDataAsync(JobSubmitInput jobInput);
    }
}
