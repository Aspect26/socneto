using System;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Domain.Models;
using Domain;

namespace Infrastructure
{
    public class KafkaProducer: IProducer
    {
        private readonly string _serverAddress;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject)
        {
            _serverAddress = kafkaOptionsObject.Value.ServerAddress;
            
        }
        public async Task ProduceAsync(string topicName, Message message)
        {
            var config = new ProducerConfig { BootstrapServers = _serverAddress };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                
                var cancelled = false;
                
                try
                {
                    // Note: Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received (at the 
                    // expense of low throughput).
                    var deliveryReport = await producer.ProduceAsync(
                        topicName, KafkaMessage.ToKafka(message)
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
