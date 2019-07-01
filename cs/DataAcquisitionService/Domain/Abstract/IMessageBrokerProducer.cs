using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IMessageBrokerProducer
    {
        Task ProduceAsync(string topic, MessageBrokerMessage message);
    }
}
