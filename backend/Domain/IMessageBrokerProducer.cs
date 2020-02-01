using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain
{
    public interface IMessageBrokerProducer
    {
        Task ProduceAsync(string channelName, Message message);
    }
}
