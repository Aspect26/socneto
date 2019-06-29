using System;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Models;

namespace Domain.Registration
{
    public class RegistrationRequestProcessor : IRegistrationRequestProcessor
    {
        private readonly ISubscribedComponentManager _subscribedComponentManager;
        private readonly IMessageBrokerApi _messageBrokerApi;

        public RegistrationRequestProcessor(
            ISubscribedComponentManager subscribedComponentManager,
            IMessageBrokerApi messageBrokerApi)
        {
            _subscribedComponentManager = subscribedComponentManager;
            _messageBrokerApi = messageBrokerApi;
        }

        public async Task ProcessRequest(RegistrationRequestMessage request)
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

            var channelCreationResult = await _messageBrokerApi.CreateChannel(channelModel);


            var componentRegisterModel = new ComponentRegistrationModel(
                request.ComponentId,
                channelCreationResult.ChannelName,
                request.ComponentType);

            _subscribedComponentManager.SubscribeComponent(componentRegisterModel);

        }
    }
}