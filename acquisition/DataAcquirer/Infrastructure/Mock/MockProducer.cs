using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.DataGenerator
{
    public class MockProducer : IMessageBrokerProducer
    {
        private class Post
        {
            public string PostId { get; set; }
        }
        private readonly ILogger<MockProducer> _logger;

        private readonly HashSet<string> _encounteredIds = new HashSet<string>();
        public MockProducer(ILogger<MockProducer> logger)
        {
            _logger = logger;
            
        }
        public Task ProduceAsync(string topic, MessageBrokerMessage message)
        {
            _logger.LogInformation("Topic {}, Message {}",
                topic,
                message.JsonPayloadPayload);

            var checkForDuplicates = true;
            if(checkForDuplicates && topic =="c1")
            {
                var uniPost = JsonConvert.DeserializeObject<Post>(message.JsonPayloadPayload);
                if (_encounteredIds.Contains(uniPost.PostId))
                {
                    _logger.LogWarning("Post with id {id} has already been processed", uniPost.PostId);
                }
                else
                {
                    _encounteredIds.Add(uniPost.PostId);
                }
            }



            return Task.CompletedTask;
        }
    }
}
