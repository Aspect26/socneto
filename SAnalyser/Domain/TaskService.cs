using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;

namespace Domain
{
    public class TaskService
    {
        private readonly IDataCollector _dataCollector;
        private readonly IAnalyser _analyser;

        public TaskService(IDataCollector dataCollector, IAnalyser analyser)
        {
            _dataCollector = dataCollector;
            _analyser = analyser;
        }

        public async Task<TaskResult> ProcessTaskAsync(TaskInput taskInput)
        {
            var collectedData = await _dataCollector.CollectDataAsync(taskInput);
            var analysedResult = await _analyser.AnalyzeAsync(collectedData);

            return new TaskResult() { Keywords = analysedResult.Keywords};
        }

        

        
    }
}
