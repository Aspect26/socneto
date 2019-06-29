using System;
using Domain.Models;

namespace Domain.ComponentManagement
{
    public class SubscribedComponentManager : ISubscribedComponentManager
    {
        private readonly IComponentRegistry _componentRegistry;

        public SubscribedComponentManager(IComponentRegistry componentRegistry)
        {
            _componentRegistry = componentRegistry;
        }
        public void SubscribeComponent(ComponentRegistrationModel componentRegistrationModel)
        {
           var registered= _componentRegistry.AddOrUpdate(componentRegistrationModel);
            if (!registered)
            {
                throw new InvalidOperationException(
                    $"Device {componentRegistrationModel.ComponentId} already exists");
            }
        }
    }

    
}
