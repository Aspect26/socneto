using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain;
using Domain.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                throw new ArgumentNullException(nameof(kafkaOptions.Value.ServerAddress));

            _serverAddress = kafkaOptions.Value.ServerAddress;
        }
        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consuming topic: '{consumeTopic}'");

            try
            {
                await ConsumeNonResilientAsync(consumeTopic, onRecieveAction, cancellationToken);
            }
            catch (JobManagementException e)
            {
                _logger.LogError("Processing message failed:{exception}", e);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Closing consumer.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ConsumeNonResilientAsync(string consumeTopic, Func<string, Task> onRecieveAction, CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _serverAddress,
                GroupId = "my-consumer-group" + Guid.NewGuid().ToString(),
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            // Note: If a key or value deserializer is not set (as is the case below), the 
            // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
            // will be used automatically (where available). The default deserializer for string
            // is UTF8. The default deserializer for Ignore returns null for all input data
            // (including non-null data).
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => _logger.LogWarning(e.Reason))
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
                while (true)
                {
                    try
                    {
                        ConsumeResult<Ignore, string> consumeResult = null;
                        do
                        {
                            consumeResult = consumer.Consume(cancellationToken);
                        } while (consumeResult.IsPartitionEOF);


                        _logger.LogTrace(
                            $"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                        try
                        {
                            await onRecieveAction(consumeResult.Value);

                        }
                        catch (ArgumentException ae)
                        {
                            _logger.LogError("Format message:'{message}',error: {errorMessage}", consumeResult.Value, ae);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Unexpected error while processing message. msg: '{message}' error:'{error}'", consumeResult.Value, e);
                            throw new JobManagementException("Could not process request", e);
                        }
                        finally
                        {
                            consumer.Commit(consumeResult);
                        }
                    }
                    catch (OperationCanceledException) { }
                }
            }


        }
    }
}
