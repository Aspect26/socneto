using Confluent.Kafka;
using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Infrastructure.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.KafkaMock
{
    public class CommandSender
    {
        private readonly IMessageBrokerProducer _kafkaProducer;
        private readonly ILogger<CommandSender> _logger;

        public CommandSender(
            IMessageBrokerProducer kafkaProducer,
            ILogger<CommandSender> logger)
        {
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        public async Task SendCommands(
            string topic,
            IEnumerable<DataAcquirerJobConfig> commands,
            TimeSpan commandDelay)
        {
            foreach (var command in commands)
            {
                var serializedConfig = JsonConvert.SerializeObject(command);
                var message = new MessageBrokerMessage("some_key", serializedConfig);
                await _kafkaProducer.ProduceAsync(topic, message);
                _logger.LogInformation("Command sent to the topic '{topic}'. Command: {cmd}", topic, command);

                await Task.Delay(commandDelay);
            }
        }
    }
}
