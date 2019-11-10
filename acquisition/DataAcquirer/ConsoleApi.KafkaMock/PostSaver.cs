using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Infrastructure.Kafka;
using Microsoft.Extensions.Logging;

namespace ConsoleApi.KafkaMock
{

    public class PostSaver
    {
        private readonly IMessageBrokerConsumer _kafkaConsumer;
        private readonly ILogger<PostSaver> _logger;
        
        public PostSaver(
            IMessageBrokerConsumer kafkaConsumer,
            ILogger<PostSaver> logger)
        {
            _kafkaConsumer = kafkaConsumer;
            _logger = logger;
            
        }


        public async Task ListenAndSaveAsync(string topic, CancellationToken cancellationToken)
        {
            _logger.LogInformation("started listening for '{topic}'", topic);
            var ts = DateTime.Now.ToString("s").Replace(":", "").Replace("-", "");
            var filePath = new FileInfo($"{topic}-{ts}.txt");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _kafkaConsumer.ConsumeAsync(topic,
                        r=>SaveAsync(r,filePath),
                        cancellationToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception e)
                {
                    _logger.LogError("Error encountered while receiving posts: {error}", e.Message);
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }

            }

        }

        private async Task SaveAsync(string json, FileInfo filePath)
        {
            _logger.LogTrace("Post saved. TS: {timestamp}, post {post}" , 
                DateTime.Now.ToString("s"),
                json);
            using (var writer = new StreamWriter(filePath.FullName, append: true))
            {
                await writer.WriteLineAsync(json);
            }
        }
    }
}
