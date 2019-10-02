using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Abstract
{
    public interface IMessageBrokerApi
    {
        Task CreateChannel(MessageBrokerChannelModel channelModel);
    }
}
