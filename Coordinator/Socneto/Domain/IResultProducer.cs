using System;
using System.Threading;
using System.Threading.Tasks;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Domain
{
    public interface IResultProducer
    {
        Task ProduceAsync( Message message);
    }
}
