using Domain.DependencyWaiting;
using Infrastructure.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Confluent.Kafka;
using System;

namespace Infrastructure.DependencyWaiting
{
    public class KafkaDependencyWaitingService : IKafkaDependencyWaitingService
    {
        private readonly IOptions<KafkaOptions> _options;
        private readonly ILogger<KafkaDependencyWaitingService> _logger;
        private readonly string _kafkaServer;

        public KafkaDependencyWaitingService(
            IOptions<KafkaOptions> options,
            ILogger<KafkaDependencyWaitingService> logger)
        {
            _options = options;
            _logger = logger;
            _kafkaServer = options.Value.ServerAddress;
        }
        public Task<bool> IsDependencyReadyAsync()
        {
            try
            {
                using (var adminClient = new AdminClientBuilder(
                    new AdminClientConfig { BootstrapServers = _kafkaServer })
                    .Build())
                {
                    adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                    return Task.FromResult( true);
                }
            }
            catch (KafkaException e)
            {
                _logger.LogWarning("Kafka is not ready yet: {error}", e);
                return Task.FromResult( false);
            }
        }
    }
}
