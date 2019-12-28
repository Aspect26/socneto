using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.ComponentManagement
{
    public interface IComponentRegistry
    {
        Task AddOrUpdateAsync(ComponentModel componentRegistrationModel);
        Task<ComponentModel> GetComponentByIdAsync(string componentId);
        StorageComponent GetRegisteredStorage();

        Task InsertJobComponentConfigAsync(JobComponentConfig jobConfig);
        Task<List<JobComponentConfig>> GetAllComponentJobConfigsAsync(string componentId);
    }
}