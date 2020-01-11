using System;
using System.IO;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.DataGenerator
{
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
        private static readonly object _lock = new object();
        private long _postEncountered = 0;

        //private readonly ConcurrentDictionary<string, int> _queryResults
        //    = new ConcurrentDictionary<string, int>();
        public FileProducer(
            IOptions<FileProducerOptions> fileProducerOptionsAccessor,
            ILogger<FileProducer> logger)
        {
            _filePath = fileProducerOptionsAccessor.Value.DestinationFilePath;
            _logger = logger;
            
        }
        public Task ProduceAsync(string topic, MessageBrokerMessage message)
        {
            _postEncountered++;
            _logger.LogInformation("Post saved. TS: {timestamp}, post {post}",
                DateTime.Now.ToString("s"),
                message.JsonPayloadPayload);

            //var post = JsonConvert.DeserializeObject<MyPost>(message.JsonPayloadPayload);

            //var lang = post.Query ?? "null";
            // _queryResults.AddOrUpdate(lang, 1, (key, value) => value + 1);

            //if (_postEncountered % 100 == 0)
            //{
            //    var ser = _queryResults.ToDictionary(r => r.Key, r => r.Value);
            //    _logger.LogInformation(JsonConvert.SerializeObject(ser));
            //}
            lock (_lock)
            {
                var file = new FileInfo(_filePath);

                using (var writer = new StreamWriter(file.FullName, append: true))
                {
                    writer.WriteLine(message.JsonPayloadPayload);
                }
            }
            return Task.CompletedTask;
        }
    }
}
