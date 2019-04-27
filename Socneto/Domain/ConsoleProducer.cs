using System;
using System.Threading.Tasks;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Domain
{
    public class ConsoleProducer : IResultProducer
    {
        public Task ProduceAsync(Message message)
        {
            Console.WriteLine("Printing produced message");
            Console.WriteLine(message.Value);
            return Task.CompletedTask;
        }
    }
}