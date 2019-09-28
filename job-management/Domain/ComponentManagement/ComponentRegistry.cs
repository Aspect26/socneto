using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            var subscribedComponent = new SubscribedComponent(
                componentRegistrationModel.ComponentId,
                componentRegistrationModel.ComponentType,
                componentRegistrationModel.InputChannelId,
                componentRegistrationModel.UpdateChannelId);



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

        public bool TryGetNetworkComponent(string componentId, out SubscribedComponent component )
        {
            const string desiredComponentName = "Network";
            return TryGetComponent(componentId, desiredComponentName,out component);
        }

        public SubscribedComponent GetRegisteredStorage()
        {
            return _registeredComponents
                .Values
                .FirstOrDefault(r => r.ComponentType == "Storage");
        }

        public IList<SubscribedComponent> GetRegisteredComponents()
        {
            return _registeredComponents.Values.ToList();
        }

        public bool TryGetAnalyserComponent(string componentId, out SubscribedComponent component)
        {
            var desiredComponentName = "Analyser";
            return TryGetComponent(componentId, desiredComponentName, out component);
        }
        
        private bool TryGetComponent(string componentId, string desiredComponentType, out SubscribedComponent component)
        {
            component = null;
            if (_registeredComponents.TryGetValue(componentId, out var val))
            {
                // TODO remove constant
                if (val.ComponentType != desiredComponentType)
                {
                    return false;
                }
                else
                {
                    component = val;
                    return true;
                }
            }
            else
            {
                return false;
            }
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
