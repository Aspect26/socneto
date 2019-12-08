using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.Registration
{
    public class RegistrationRequestListener
    {
        private readonly IRegistrationRequestProcessor _registrationRequestProcessor;
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly ILogger<RegistrationRequestListener> _logger;
        private readonly string _registrationChannelName;

        public RegistrationRequestListener(IRegistrationRequestProcessor registrationRequestProcessor,
            IMessageBrokerConsumer messageBrokerConsumer,
            IOptions<RegistrationRequestOptions> requestListenerOptionsAccessor,
            ILogger<RegistrationRequestListener> logger)
        {
            _registrationRequestProcessor = registrationRequestProcessor;
            _messageBrokerConsumer = messageBrokerConsumer;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(requestListenerOptionsAccessor.Value.RegistrationChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name",
                    nameof(requestListenerOptionsAccessor.Value.RegistrationChannelName));
            }

            _registrationChannelName = requestListenerOptionsAccessor.Value.RegistrationChannelName;
        }

        public async Task Listen(CancellationToken cancellationToken)
        {
                    await _messageBrokerConsumer.ConsumeAsync(
                        _registrationChannelName,
                        ProcessRegistrationRequest,
                        cancellationToken);
                
            
        }

        private async Task ProcessRegistrationRequest(string registrationRequest)
        {

            _logger.LogInformation("Registration request accepted: {registrationRequestJson}",
                registrationRequest);

            RegistrationRequestMessage registrationRequestMessage = null;
            try
            {
                registrationRequestMessage = JsonConvert.DeserializeObject<RegistrationRequestMessage>(registrationRequest);
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError("Error while parsing registration request: {error}", jre.Message);
                return;
            }
            try
            {

                await _registrationRequestProcessor.ProcessRequestAsync(registrationRequestMessage);
            }
            catch (ArgumentException ae)
            {
                _logger.LogError("Format error: {errorMessage}", ae.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error: {errorMessage}", e.Message);
            }
        }
    }
}