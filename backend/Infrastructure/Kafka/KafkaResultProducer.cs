using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Domain;
using Socneto.Domain.Models;

namespace Socneto.Infrastructure.Kafka
{
    public class MockResultProducer : IResultProducer
    {
        private readonly ILogger<MockResultProducer> _logger;

        public MockResultProducer(ILogger<MockResultProducer> logger)
        {
            _logger = logger;
        }

        public Task ProduceAsync(Message message)
        {
            _logger.LogInformation("Topic {}, Message {}",
                message.Key,
                message.Value);

            return Task.CompletedTask;
        }
    }
    public class KafkaResultProducer : IResultProducer
    {
        private readonly ILogger<KafkaResultProducer> _logger;
        private readonly string _serverAddress;

        private readonly string _produceTopic;

        public KafkaResultProducer(IOptions<KafkaOptions> kafkaOptionsObject, IOptions<TaskOptions> taskOptionObject, ILogger<KafkaResultProducer> logger)
        {

            if (string.IsNullOrEmpty(kafkaOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(kafkaOptionsObject.Value.ServerAddress));
            _logger = logger;
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
