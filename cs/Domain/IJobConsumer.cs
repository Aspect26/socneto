using System;
using System.Threading;
using System.Threading.Tasks;

namespace Socneto.Domain
{
    public interface IJobConsumer
    {
        Task ConsumeAsync(Func<string, Task> onRecieveAction, CancellationToken cancellationToken);
    }
}