using Domain.Models;

namespace Domain.ComponentManagement
{
    public interface IComponentRegistry
    {
        bool AddOrUpdate(ComponentRegistrationModel componentRegistrationModel);
    }
}