using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Socneto.DataAcquisition.Domain;

namespace Socneto.DataAcquisition.Infrastructure.Kafka
{
    public class KafkaProducer:IResultProducer
    {
        private readonly string _serverAddress;
        
        private readonly string _produceTopic;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject, IOptions<TaskOptions> taskOptionObject)
        {

            if (string.IsNullOrEmpty(kafkaOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ServerAddress));
                _serverAddress = kafkaOptionsObject.Value.ServerAddress;



            if (string.IsNullOrEmpty(taskOptionObject.Value.ProduceTopic))
                throw new ArgumentNullException(nameof(taskOptionObject.Value.ProduceTopic));

            _produceTopic = taskOptionObject.Value.ProduceTopic;


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
                        _produceTopic, KafkaMessage.ToKafka(message)
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
