using System;
using System.Threading.Tasks;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.Registration
{
    public class RegistrationService:IRegistrationService
    {
        private readonly IMessageBrokerProducer _messageBrokerProducer;
        private readonly string _registrationChannelName;

        public RegistrationService(IMessageBrokerProducer messageBrokerProducer,
            IOptions<RegistrationRequestOptions> registrationServiceOptionsAccessor
        )
        {
            _messageBrokerProducer = messageBrokerProducer;
            if (string.IsNullOrEmpty(registrationServiceOptionsAccessor.Value.RegistrationChannelName))
            {
                throw new ArgumentException(
                    "Argument must be valid channel name",
                    nameof(registrationServiceOptionsAccessor.Value.RegistrationChannelName));
            }

            _registrationChannelName = registrationServiceOptionsAccessor.Value.RegistrationChannelName;
        }

        public async Task Register(RegistrationRequest registrationRequest)
        {
            var registrationRequestJson = JsonConvert.SerializeObject(registrationRequest);
            var messageBrokerMessage = new MessageBrokerMessage(
                "registration-key",
                registrationRequestJson);

            await _messageBrokerProducer.ProduceAsync(
                _registrationChannelName,
                messageBrokerMessage);
        }
    }
}