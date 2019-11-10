using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Kafka
{
    public class KafkaConsumer : IMessageBrokerConsumer
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly string _serverAddress;


        public KafkaConsumer(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaConsumer> logger)
        {
            _logger = logger;

            if (string.IsNullOrEmpty(kafkaOptions.Value.ServerAddress))
            {
                throw new ArgumentNullException(nameof(kafkaOptions.Value.ServerAddress));
            }

            _serverAddress = kafkaOptions.Value.ServerAddress;
        }
        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consuming topic: '{consumeTopic}'");

            var config = new ConsumerConfig
            {
                BootstrapServers = _serverAddress,
                GroupId = "my-consumer-group",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            const int commitPeriod = 5;

            // Note: If a key or value deserializer is not set (as is the case below), the 
            // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
            // will be used automatically (where available). The default deserializer for string
            // is UTF8. The default deserializer for Ignore returns null for all input data
            // (including non-null data).
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError(e.Reason))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.LogTrace($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    // 
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.LogTrace($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(consumeTopic);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);

                            if (consumeResult.IsPartitionEOF)
                            {
                                continue;
                            }

                            _logger.LogTrace(
                                $"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                            await onRecieveAction(consumeResult.Value);

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                // The Commit method sends a "commit offsets" request to the Kafka
                                // cluster and synchronously waits for the response. This is very
                                // slow compared to the rate at which the consumer is capable of
                                // consuming messages. A high performance application will typically
                                // commit offsets relatively infrequently and be designed handle
                                // duplicate messages in the event of failure.
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    _logger.LogError($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Closing consumer.");
                    consumer.Close();
                }
            }
        }
    }
}
