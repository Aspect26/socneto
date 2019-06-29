using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain;
using Domain.Abstract;

namespace Infrastructure.Kafka
{
    public class KafkaProducer : IMessageBrokerProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly string _serverAddress;
        
        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject, ILogger<KafkaProducer> logger)
        {

            if (string.IsNullOrEmpty(kafkaOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ServerAddress));
            _logger = logger;
            _serverAddress = kafkaOptionsObject.Value.ServerAddress;
            
        }
        public async Task ProduceAsync(string topic, MessageBrokerMessage message)
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
                        topic, KafkaMessage.FromMessageBrokerMessage(message)
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
