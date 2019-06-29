using System;
using Domain.Models;

namespace Domain
{
    public class RegistrationRequestProcessor : IRegistrationRequestProcessor
    {
        private readonly IComponentConfigSubscriber _componentConfigSubscriber;
        private readonly IMessageBrokerApi _messageBrokerApi;

        public RegistrationRequestProcessor(
            IComponentConfigSubscriber componentConfigSubscriber,
            IMessageBrokerApi messageBrokerApi)
        {
            _componentConfigSubscriber = componentConfigSubscriber;
            _messageBrokerApi = messageBrokerApi;
        }

        public void ProcessRequest(RegistrationRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(request.ComponentId))
            {
                throw new ArgumentException("Argument must be valid component name"
                    , nameof(request.ComponentId));
            }

            if (string.IsNullOrWhiteSpace(request.ComponentType))
            {
                throw new ArgumentException("Argument must be valid component type name"
                    , nameof(request.ComponentType));
            }
            


            var channelModel = MessageBrokerChannelModel.FromRequest(request);

            var channelCreationResult = _messageBrokerApi.CreateChannel(channelModel);


            var componentRegisterModel = new ComponentRegisterModel(
                channelCreationResult.ChannelName,
                request.ComponentType);

            _componentConfigSubscriber.SubscribeComponent(componentRegisterModel);

        }
    }
}