using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IMessageBrokerProducer
    {
        Task ProduceAsync(string channelName, MessageBrokerMessage message);
    }
}