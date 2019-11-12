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
        private readonly string _topicQuery;
        private readonly IReadOnlyDictionary<string,string> _customAttributes;

        public MockConsumer(
            IOptions<MockConsumerOptions> mockConsumerOptionsAccessor,
            ILogger<MockConsumer> logger)
        {
            _logger = logger;
            _consumedTopic = mockConsumerOptionsAccessor.Value.ConsumedTopic;
            _topicQuery = mockConsumerOptionsAccessor.Value.TopicQuery;
            _customAttributes = mockConsumerOptionsAccessor.Value.CustomAttributes?? new Dictionary<string, string>();
        }

        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Topic {}", consumeTopic);

            if (consumeTopic == _consumedTopic)
            {
                var attributes = _customAttributes.ToDictionary(r => r.Key, r => r.Value);
                attributes.Add("TopicQuery", _topicQuery);

                var fixedGuid = Guid.Parse("01c3ee17-c9f4-492f-ac9c-e9f6ecd1fa7e");
                var config = new DataAcquirerJobConfig()
                {
                    JobId = fixedGuid,
                    Attributes = attributes,
                    Command = "start",
                    OutputMessageBrokerChannels = new string[] { "s1" }
                };


                var json = JsonConvert.SerializeObject(config);
                await onRecieveAction(json);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            await Task.Delay(TimeSpan.MaxValue);
        }
    }
}
