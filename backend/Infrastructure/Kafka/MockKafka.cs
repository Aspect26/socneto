using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Socneto.Domain;
using Socneto.Domain.Models;

namespace Socneto.Infrastructure.Kafka
{
    public class MockKafka : IMessageBrokerProducer
    {
        private readonly ILogger<MockKafka> _logger;

        public MockKafka(ILogger<MockKafka> logger)
        {
            _logger = logger;
        }

        private static Guid _jobId = Guid.NewGuid();
        
        public Task ProduceAsync(string topic, Message message)
        {
            _logger.LogInformation("Topic {}, Message {}",
                topic,
                message.Value);

            return Task.CompletedTask;
        }
    }
}

