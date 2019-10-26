using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.ComponentManagement
{
    public interface IComponentRegistry
    {
        Task<bool> AddOrUpdateAsync(ComponentRegistrationModel componentRegistrationModel);
        Task<SubscribedComponent> GetComponentById(string componentId);
        StorageComponent GetRegisteredStorage();
    }
}