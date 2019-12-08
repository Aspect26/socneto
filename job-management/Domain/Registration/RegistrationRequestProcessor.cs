using System.Linq;
using System;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Domain.Registration
{
    public class RegistrationRequestProcessor : IRegistrationRequestProcessor
    {
        private readonly ComponentIdentifiers _componentIdentifiers;
        private readonly ISubscribedComponentManager _subscribedComponentManager;
        private readonly IMessageBrokerApi _messageBrokerApi;
        private readonly RegistrationRequestValidationOptions _registrationRequestValidation;
        private readonly ILogger<RegistrationRequestProcessor> _logger;

        public RegistrationRequestProcessor(
                ISubscribedComponentManager subscribedComponentManager,
                IMessageBrokerApi messageBrokerApi,
                IOptions<ComponentIdentifiers> componentIdentifierOptions,
                IOptions<RegistrationRequestValidationOptions> registrationRequestValidationOptions,
                ILogger<RegistrationRequestProcessor> logger)
        {
            _componentIdentifiers = componentIdentifierOptions.Value;
            _subscribedComponentManager = subscribedComponentManager;
            _messageBrokerApi = messageBrokerApi;
            _registrationRequestValidation = registrationRequestValidationOptions.Value;
            _logger = logger;
        }

        public async Task ProcessRequestAsync(RegistrationRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(request.ComponentId))
            {
                throw new ArgumentException(
                    "Argument must be valid component name"
                    , nameof(request.ComponentId));
            }

            if (string.IsNullOrWhiteSpace(request.ComponentType))
            {
                throw new ArgumentException("Component type must not be empty"
                    , nameof(request.ComponentType));
            }
            else if (request.ComponentType != _componentIdentifiers.AnalyserComponentTypeName
                && request.ComponentType != _componentIdentifiers.DataAcquirerComponentTypeName)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request.ComponentType),
                    "Component type must be valid component identifier");
            }
            

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

            ValidateAttributes(request.ComponentType, request.Attributes);

            var componentRegisterModel = new ComponentModel(
                request.ComponentId,
                request.ComponentType,
                request.InputChannelName,
                request.UpdateChannelName,
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
                _logger.LogError("Unexpected error while processing request: {errorMessage}", e.Message);
            }

            await _messageBrokerApi.CreateChannel(channelModel);

        }

        private void ValidateAttributes(
            string componentType,
            JObject attributes)
        {

            if (componentType == _componentIdentifiers.AnalyserComponentTypeName)
            {
                ValidateDataAnalyser(attributes);
            }
            else if (componentType == _componentIdentifiers.DataAcquirerComponentTypeName)
            {
                ValidateDataAcquirer(attributes);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(componentType),
                    $"Unknown component type {componentType}");
            }
        }

        private void ValidateDataAnalyser(JObject attributes)
        {
            var formatElement = _registrationRequestValidation.AnalyserOutputFormatElementName;
            if (attributes == null || !attributes.ContainsKey(formatElement))
            {
                var formatError = $"Data analyser registration request must contain: '{formatElement}' element";
                throw new FormatException(formatError);
            }
        }

        private void ValidateDataAcquirer( JObject attributes)
        {
            // nothing to validate yet
        }
    }
}
