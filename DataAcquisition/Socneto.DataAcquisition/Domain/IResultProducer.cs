using System;
using System.Threading;
using System.Threading.Tasks;
using Socneto.DataAcquisition.Domain;

namespace Socneto.DataAcquisition.Domain
{
    public interface IResultProducer
    {
        Task ProduceAsync( Message message);
    }

    public interface IJobConsumer
    {
        Task ConsumeAsync(Func<string, Task> onRecieveAction, CancellationToken cancellationToken);
    }
}
