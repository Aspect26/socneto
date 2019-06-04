using System;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain
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