using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Abstract
{
    public interface IMessageBrokerApi
    {
        Task<CreateChannelResult> CreateChannel(MessageBrokerChannelModel channelModel);
    }
}
