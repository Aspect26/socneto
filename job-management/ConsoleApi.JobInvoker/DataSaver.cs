using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Infrastructure.Kafka;
using Microsoft.Extensions.Logging;

namespace ConsoleApi.JobInvoker
{
    public class DataSaver
    {
        private readonly IMessageBrokerConsumer _kafkaConsumer;
        private readonly ILogger<DataSaver> _logger;
        
        public DataSaver(
            IMessageBrokerConsumer kafkaConsumer,
            ILogger<DataSaver> logger)
        {
            _kafkaConsumer = kafkaConsumer;
            _logger = logger;            
        }


        public async Task ListenAndSaveAsync(string topic, 
            DirectoryInfo directory, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("started listening for '{topic}'", topic);
            var ts = DateTime.Now.ToString("s").Replace(":", "").Replace("-", "");
            var filepathText = Path.Combine(directory.FullName, $"{topic}-{ts}.txt");
            var filePath = new FileInfo(filepathText);
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
            _logger.LogTrace("DataObjectSaved saved. TS: {timestamp}, post {post}" , 
                DateTime.Now.ToString("s"),
                json);
            using (var writer = new StreamWriter(filePath.FullName, append: true))
            {
                await writer.WriteLineAsync(json);
            }
        }
    }
}
