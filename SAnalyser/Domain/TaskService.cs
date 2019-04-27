using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain
{
    public class TaskService : ITaskService
    {
        private readonly IDataCollector _dataCollector;

        private readonly IConsumer _consumer;
        private readonly IProducer _producer;
        private readonly ILogger<TaskService> _logger;
        private readonly TaskOptions _taskOptions;
        private Task _consumeTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public TaskService(IDataCollector dataCollector, IConsumer consumer, IProducer producer, IOptions<TaskOptions> taskOptions, ILogger<TaskService> logger)
        {
            _dataCollector = dataCollector;

            _consumer = consumer;
            _producer = producer;
            _logger = logger;
            _taskOptions = taskOptions.Value;
        }

        public void StartTaskConsumeTasks()
        {
            _logger.LogInformation("Starting to consume tasks");
            _consumeTask = _consumer.ConsumeAsync(
                _taskOptions.ConsumeTaskTopic,
                Process,
                _cancellationTokenSource.Token
                );
        }

        private async Task Process(string taskJson)
        {

            try
            {

                var taskInput = JsonConvert.DeserializeObject<TaskInput>(taskJson);

                var page = await _dataCollector.CollectDataAsync(taskInput);

                _logger.LogInformation("Processing data");
                foreach (var post in page.PostDataList)
                {
                    var json = JsonConvert.SerializeObject(post);
                    await _producer.ProduceAsync(_taskOptions.ProduceDbStoreTopic,
                        new Message()
                        {
                            Key = "Post",
                            Value = json
                        }
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task StopTaskConsumeTaskAsync()
        {
            _logger.LogInformation("Canceling task consumption");
            _cancellationTokenSource.Cancel();
            await _consumeTask;
        }

    }
}
