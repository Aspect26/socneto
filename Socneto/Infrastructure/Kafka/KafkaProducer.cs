using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Socneto.Coordinator.Domain;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Infrastructure.Kafka
{
    public class KafkaProducer: IResultProducer
    {
        private readonly string _serverAddress;
        private readonly string _topicName;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject)
        {
            _serverAddress = kafkaOptionsObject.Value.ServerAddress;
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

                    Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }

}
