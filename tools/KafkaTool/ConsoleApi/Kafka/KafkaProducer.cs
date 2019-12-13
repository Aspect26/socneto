using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Kafka
{
    public class KafkaProducer
    {
        private readonly string _serverAddress;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptionsObject)
        {

            if (string.IsNullOrEmpty(kafkaOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ServerAddress));
            
            _serverAddress = kafkaOptionsObject.Value.ServerAddress;

        }
        public async Task ProduceAsync(string channelName, Message<string,string> message)
        {
            var config = new ProducerConfig { BootstrapServers = _serverAddress };

            var producerBuilder = new ProducerBuilder<string, string>(config);
            producerBuilder.SetErrorHandler(
                (producer, error) => Console.Error.WriteLine(error)
            );


            using (var producer = producerBuilder.Build())
            {

                try
                {
                    // Note: Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received (at the 
                    // expense of low throughput).

                    var deliveryReport = await producer.ProduceAsync(
                        channelName, message
                        );

                    //producer.Produce(
                    //    channelName, KafkaMessage.FromMessageBrokerMessage(message)
                    //);


                    //_logger.LogInformation($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    
                }
            }
        }
    }
}
