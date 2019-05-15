using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Coordinator.Domain;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Infrastructure.Kafka
{
    public class KafkaProducer: IResultProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly string _serverAddress;
        private readonly string _topicName;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject, ILogger<KafkaProducer> logger)
        {
            _logger = logger;
            if(string.IsNullOrEmpty(kafkaOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ServerAddress));
            _serverAddress = kafkaOptionsObject.Value.ServerAddress;
            
            if (string.IsNullOrEmpty(kafkaOptionsObject.Value.ProduceDbStoreTopic))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ProduceDbStoreTopic));
            _topicName = kafkaOptionsObject.Value.ProduceDbStoreTopic;

        }

        public async Task ProduceAsync(Message message)
        {
            var config = new ProducerConfig { BootstrapServers = _serverAddress };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    // Note: Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received (at the 
                    // expense of low throughput).
                    var deliveryReport = await producer.ProduceAsync(
                        _topicName, KafkaMessage.ToKafka(message)
                        );

                    _logger.LogInformation($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    _logger.LogError($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }

}
