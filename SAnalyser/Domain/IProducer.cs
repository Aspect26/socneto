using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain
{
    public interface IProducer
    {
        Task ProduceAsync(string topicName, Message message);
    }

    public interface IConsumer
    {
        Task ConsumeAsync(string topic, Func<string, Task> onRecieveAction, CancellationToken cancellationToken);
    }
}
