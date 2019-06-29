using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain
{
    

    public interface IMessageBrokerConsumer
    {
        Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken);
    }
   

    
    
    public interface IMessageBrokerApi
    {
        Task<CreateChannelResult> CreateChannel(MessageBrokerChannelModel channelModel);

    }
    
    public interface ISubscribedComponentManager
    {
        void SubscribeComponent(ComponentRegistrationModel componentRegistrationModel);

        // todo
        // Task PushJobConfiguration();
    }
    

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

    public interface IComponentRegistry
    {
        bool AddOrUpdate(ComponentRegistrationModel componentRegistrationModel);
    }

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
