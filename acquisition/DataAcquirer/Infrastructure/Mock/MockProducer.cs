using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private object _ioLock = new object();

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
            if (checkForDuplicates && topic == "s1")
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
                lock (_ioLock)
                {
                    //using (var writer = new StreamWriter("outIds.txt"))
                    //{
                    //    var ids = new
                    //    {
                    //        Ids = _encounteredIds
                    //    };
                    //    var idsJson = JsonConvert.SerializeObject(ids);
                    //    writer.WriteLineAsync(idsJson);
                    //}
                }
            }



            return Task.CompletedTask;
        }
    }
}
