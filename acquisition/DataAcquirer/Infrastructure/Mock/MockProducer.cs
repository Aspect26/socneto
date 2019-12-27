using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.DataGenerator
{
    public class FileProducerOptions
    {
        [Required]
        public string DestinationFilePath { get; set; }
    }

    public class FileProducer : IMessageBrokerProducer
    {
        private class MyPost
        {
            [JsonProperty("language")]
            public string Language { get; set; }

            [JsonProperty("query")]
            public string Query { get; set; }
        }
        private readonly string _filePath;
        private readonly ILogger<FileProducer> _logger;
        private long _postEncountered = 0;


        private readonly ConcurrentDictionary<string, int> _queryResults
            = new ConcurrentDictionary<string, int>();
        public FileProducer(
            IOptions<FileProducerOptions> fileProducerOptionsAccessor,
            ILogger<FileProducer> logger)
        {
            _filePath = fileProducerOptionsAccessor.Value.DestinationFilePath;
            _logger = logger;
        }
        public async Task ProduceAsync(string topic, MessageBrokerMessage message)
        {
            _postEncountered++;
            _logger.LogTrace("Post saved. TS: {timestamp}, post {post}",
                DateTime.Now.ToString("s"),
                message.JsonPayloadPayload);

            var post = JsonConvert.DeserializeObject<MyPost>(message.JsonPayloadPayload);

            var lang = post.Query ?? "null";
            _queryResults.AddOrUpdate(lang, 1, (key, value) => value + 1);

            if (_postEncountered % 100 == 0)
            {
                var ser = _queryResults.ToDictionary(r => r.Key, r => r.Value);
                _logger.LogInformation(JsonConvert.SerializeObject(ser));
            }

            using (var writer = new StreamWriter(_filePath, append: true))
            {
                await writer.WriteLineAsync(message.JsonPayloadPayload);
            }
        }
    }

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
