using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain;
using Domain.Abstract;
using Newtonsoft.Json;

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
        public async Task ProduceAsync(string channelName, MessageBrokerMessage message)
        {
            var config = new ProducerConfig { BootstrapServers = _serverAddress };

            var producerBuilder = new ProducerBuilder<string, string>(config);
            producerBuilder.SetErrorHandler(
                (producer, error) => { _logger.LogError(error.Reason); }
            );


            using (var producer = producerBuilder.Build())
            {

                try
                {
                    // Note: Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received (at the 
                    // expense of low throughput).

                    var deliveryReport = await producer.ProduceAsync(
                        channelName, KafkaMessage.FromMessageBrokerMessage(message)
                        );

                    //producer.Produce(
                    //    channelName, KafkaMessage.FromMessageBrokerMessage(message)
                    //);


                    //_logger.LogInformation($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    _logger.LogError($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }

    public class MockKafka : IMessageBrokerConsumer,IMessageBrokerProducer
    {
        private readonly ILogger<MockKafka> _logger;

        public MockKafka(ILogger<MockKafka> logger)
        {
            _logger = logger;
        }

        private static Guid _jobId = Guid.NewGuid();
        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Topic {}", consumeTopic);


            //if (consumeTopic == "job_management.job_configuration.DataAnalyzer_Mock")
            //{
            //    //var config = new DataAcquirerJobConfig()
            //    //{
            //    //    JobId = Guid.NewGuid(),
            //    //    Attributes = new Dictionary<string, string>
            //    //    {
            //    //        {"TopicQuery","FooBar"}
            //    //    },
            //    //    OutputMessageBrokerChannels = new string[] { "s1", "a1", "a2" }
            //    //};
            //    var config = new DataAnalyzerJobConfig()
            //    {
            //        JobId = _jobId,
            //        OutputChannelName = "Storage_output_channel"
            //    };
            //    var json = JsonConvert.SerializeObject(config);
            //    await onRecieveAction(json);
            //}
            //if (consumeTopic == "job_management.component_data_input.DataAnalyzer_Mock")
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //    while (true)
            //    {
            //        await Task.Delay(TimeSpan.FromSeconds(10));

            //        var config = UniPost.FromValues(
            //            Guid.NewGuid().ToString(),
            //            "RANDOM text",
            //        "test souirce",
            //            DateTime.Now.ToString("s"),
            //            "foobarUser",
            //            _jobId
            //        );

            //        var json = JsonConvert.SerializeObject(config);
            //        await onRecieveAction(json);
            //    }
            //}

            await Task.Delay(TimeSpan.FromMinutes(10));
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
