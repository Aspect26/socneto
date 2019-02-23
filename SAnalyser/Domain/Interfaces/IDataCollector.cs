using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IDataCollector
    {
        Task<DataContainer> CollectDataAsync(TaskInput taskInput);
    }
}
