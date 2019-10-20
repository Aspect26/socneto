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

            if (string.IsNullOrWhiteSpace(request.UpdateChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name"
                    , nameof(request.UpdateChannelName));
            }

            if (string.IsNullOrWhiteSpace(request.InputChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name"
                    , nameof(request.InputChannelName));
            }

            var channelModel = MessageBrokerChannelModel.FromRequest(request);
            
            var componentRegisterModel = new ComponentRegistrationModel(
                request.ComponentId,
                request.UpdateChannelName,
                request.InputChannelName,
                request.ComponentType,
                request.Attributes);
            try
            {
                var subscribeComponentModel = await _subscribedComponentManager
                    .SubscribeComponentAsync(componentRegisterModel);
                
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error while processing request: {errorMessage}",e.Message);
            }

            await _messageBrokerApi.CreateChannel(channelModel);

        }
    }
}