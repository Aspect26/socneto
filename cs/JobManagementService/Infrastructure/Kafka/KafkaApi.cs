using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka
{
    public class KafkaApi : IMessageBrokerApi
    {
        private readonly ILogger<KafkaApi> _logger;

        public KafkaApi(ILogger<KafkaApi> logger)
        {
            _logger = logger;
        }
        public Task<CreateChannelResult> CreateChannel(MessageBrokerChannelModel channelModel)
        {
            _logger.LogWarning("CreateChannel did nothing");
            return Task.FromResult(new CreateChannelResult("foo.bar"));
        }
    }
}