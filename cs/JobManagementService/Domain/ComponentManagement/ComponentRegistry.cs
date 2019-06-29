using System.Collections.Concurrent;
using System.Collections.Generic;
using Domain.Models;

namespace Domain.ComponentManagement
{
    public class ComponentRegistry :IComponentRegistry
    {
        private readonly ConcurrentDictionary<string, List<SubscribedComponent>> _registeredComponentsTypes 
            = new ConcurrentDictionary<string, List<SubscribedComponent>>();

        private readonly ConcurrentDictionary<string, SubscribedComponent> _registeredComponents
            =new ConcurrentDictionary<string, SubscribedComponent>();

        public bool AddOrUpdate(ComponentRegistrationModel componentRegistrationModel)
        {
            var subscribedComponent = new SubscribedComponent(componentRegistrationModel.ComponentType,
                componentRegistrationModel.ChannelId);

            if (!_registeredComponents.TryAdd(componentRegistrationModel.ComponentId, subscribedComponent))
            {
                // Already exists
                return false;
            }
            
            var key = componentRegistrationModel.ComponentType;
            _registeredComponentsTypes.AddOrUpdate(key,
                k => new List<SubscribedComponent>() { subscribedComponent},
                (k, existingList) =>
                {
                    existingList.Add(subscribedComponent);
                    return existingList;
                });
            return true;
        }

        

        public IList<SubscribedComponent> GetAllByType(string componentType)
        {
            if (_registeredComponentsTypes.TryGetValue(componentType, out var list))
            {
                return list;
            }
            else
            {
                return new List<SubscribedComponent>();
            }
        }
    }

    
}
