using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.SubmittedJobConfiguration
{
    public class ComponentConfigUpdateNotifier:IComponentConfigUpdateNotifier
    {
        private readonly IMessageBrokerProducer _messageBrokerProducer;
        private readonly ILogger<ComponentConfigUpdateNotifier> _logger;

        public ComponentConfigUpdateNotifier(
            IMessageBrokerProducer messageBrokerProducer,
            ILogger<ComponentConfigUpdateNotifier> logger
        )
        {
            _messageBrokerProducer = messageBrokerProducer;
            _logger = logger;
        }
        public async Task NotifyComponentAsync(string componentConfigChannelName, object notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var message = new MessageBrokerMessage("job-config-notification",json);
            _logger.LogInformation("Sending message {messageJson}", json);

            await _messageBrokerProducer.ProduceAsync(componentConfigChannelName,message);
        }
    }
}