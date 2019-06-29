using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;

namespace Tests.MockClasses
{
    public class MessageBrokerConsumerMock : IMessageBrokerConsumer
    {
        public string ConsumeTopic { get; set; }
        public Func<string, Task> OnRecieveAction { get; private set; }
        public async Task ConsumeAsync(string consumeTopic, Func<string, Task> onRecieveAction, CancellationToken cancellationToken)
        {
            ConsumeTopic = consumeTopic;
            OnRecieveAction = onRecieveAction;

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
