using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain
{
    public interface IResultProducer
    {
        Task ProduceAsync( Message message);
    }

    public interface IMessageBrokerProducer
    { }

}
