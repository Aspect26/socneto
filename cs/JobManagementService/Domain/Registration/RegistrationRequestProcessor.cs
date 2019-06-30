using System.Linq;
using System;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Registration
{
    public class RegistrationRequestProcessor : IRegistrationRequestProcessor
    {
        private readonly ISubscribedComponentManager _subscribedComponentManager;
        private readonly IMessageBrokerApi _messageBrokerApi;
        private readonly ILogger<RegistrationRequestProcessor> _logger;


        private readonly string[] _availableComponentsTypes = new[] { "Network", "Analyser"};

        public RegistrationRequestProcessor(
                ISubscribedComponentManager subscribedComponentManager,
                IMessageBrokerApi messageBrokerApi,
                ILogger<RegistrationRequestProcessor> logger)
        {
            _subscribedComponentManager = subscribedComponentManager;
            _messageBrokerApi = messageBrokerApi;
            _logger = logger;
        }

        public async Task ProcessRequestAsync(RegistrationRequestMessage request)
        {

            if (string.IsNullOrWhiteSpace(request.ComponentId))
            {
                throw new ArgumentException("Argument must be valid component name"
                    , nameof(request.ComponentId));
            }

            if (string.IsNullOrWhiteSpace(request.ComponentType)
            || !_availableComponentsTypes.Contains(request.ComponentType))
            {

                throw new ArgumentException("Argument must be valid component type name"
                    , nameof(request.ComponentType));
            }
            // TODO check if it is not already registered
            
            var channelModel = MessageBrokerChannelModel.FromRequest(request);
            var channelCreationResult = await _messageBrokerApi.CreateChannel(channelModel);
            
            var componentRegisterModel = new ComponentRegistrationModel(
                request.ComponentId,
                channelCreationResult.ChannelName,
                request.ComponentType);
            try
            {
                _subscribedComponentManager.SubscribeComponent(componentRegisterModel);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error while processing request: {errorMessage}",e.Message);
            }

        }
    }
}