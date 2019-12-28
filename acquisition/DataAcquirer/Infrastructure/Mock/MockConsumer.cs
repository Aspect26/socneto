using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.JobConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.DataGenerator
{
    public class MockConsumer : IMessageBrokerConsumer
    {
        private readonly ILogger<MockConsumer> _logger;
        private readonly string _consumedTopic;
        private readonly IReadOnlyDictionary<string,string> _customAttributes;
        private readonly Queue<DataAcquirerJobConfig> _configs;
        
        public MockConsumer(
            IOptions<MockConsumerOptions> mockConsumerOptionsAccessor,
            ILogger<MockConsumer> logger)
        {
            _logger = logger;
            _consumedTopic = mockConsumerOptionsAccessor.Value.ConsumedTopic;
            _customAttributes = mockConsumerOptionsAccessor.Value.CustomAttributes?? new Dictionary<string, string>();
            
            _configs = PrepareConfigQueue(mockConsumerOptionsAccessor.Value.Topics);
        }

        public Queue<DataAcquirerJobConfig> GetFixed()
        {
            var attributes = _customAttributes.ToDictionary(r => r.Key, r => r.Value);
            attributes.Add("TopicQuery", "?");

            var fixedGuid = Guid.Parse("01c3ee17-c9f4-492f-ac9c-e9f6ecd1fa7e");
            var config= new DataAcquirerJobConfig()
            {
                JobId = fixedGuid,
                Attributes = attributes,
                Command = JobCommand.Start,
                OutputMessageBrokerChannels = new string[] { "s1" }
            };
            var queue = new Queue<DataAcquirerJobConfig>();
            queue.Enqueue(config);
            return queue;
        }

        public Queue<DataAcquirerJobConfig> PrepareConfigQueue(IEnumerable<string> topics)
        {
            var configs = new Queue<DataAcquirerJobConfig>();

            foreach(var topic in topics)
            {
                var attributes = _customAttributes.ToDictionary(r => r.Key, r => r.Value);
                attributes.Add("TopicQuery",topic );

                var config = new DataAcquirerJobConfig()
                {
                    JobId = Guid.NewGuid(),
                    Attributes = attributes,
                    Command = JobCommand.Start,
                    OutputMessageBrokerChannels = new string[] { "o_1" }
                };

                configs.Enqueue(config);

            }

            return configs;
        }

        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consuming topic {}", consumeTopic);

            if (consumeTopic == _consumedTopic)
            {
                var config = _configs.Dequeue();
                var json = JsonConvert.SerializeObject(config);
                await onRecieveAction(json);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
