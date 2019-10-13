using System;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataGenerator
{
    public class MockProducer : IMessageBrokerProducer
    {
        private readonly ILogger<MockProducer> _logger;

        public MockProducer(ILogger<MockProducer> logger)
        {
            _logger = logger;
        }
        public Task ProduceAsync(string topic, MessageBrokerMessage message)
        {
            _logger.LogInformation("Topic {}, Message {}",
                topic,
                message.JsonPayloadPayload);

            return Task.CompletedTask;
        }
    }
}
