using System.Collections.Generic;
using Domain.Models;

namespace Domain.ComponentManagement
{
    public interface IComponentRegistry
    {
        bool AddOrUpdate(ComponentRegistrationModel componentRegistrationModel);

        bool TryGetAnalyserComponent(string componentId, out SubscribedComponent component);
        bool TryGetNetworkComponent(string componentId, out SubscribedComponent component);

        IList<SubscribedComponent> GetRegisteredComponents();
    }
}