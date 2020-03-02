using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Domain.Services
{
    public class ChartsService : IChartsService
    {

        private readonly IStorageService _storageService;

        public ChartsService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public async Task<IList<ChartDefinition>> GetJobCharts(Guid jobId)
        {
            var jobView = await _storageService.GetJobView(jobId);
            return jobView?.ViewConfiguration?.ChartDefinitions ?? new List<ChartDefinition>();
        }

        public async Task<ChartDefinition> CreateJobChart(Guid jobId, string title, ChartType chartType, List<AnalysisDataPath> analysisDataPaths, bool isXPostDateTime)
        {
            var newChartDefinition = new ChartDefinition
            {
                Identifier = Guid.NewGuid(),
                Title = title,
                ChartType = chartType,
                AnalysisDataPaths = analysisDataPaths,
                IsXPostDatetime = isXPostDateTime
            };

            var jobView = await _storageService.GetJobView(jobId);
            EnsureCorrectViewConfiguration(jobView);
            jobView.ViewConfiguration.ChartDefinitions.Add(newChartDefinition);
            
            await _storageService.UpdateJobView(jobId, jobView);

            return newChartDefinition;
        }

        public async Task<JobView> RemoveJobChart(Guid jobId, Guid chartId)
        {
            var jobView = await _storageService.GetJobView(jobId);

            var chart = jobView?.ViewConfiguration?.ChartDefinitions?.SingleOrDefault(definition => definition.Identifier == chartId);
            if (chart == null) return null;

            jobView.ViewConfiguration.ChartDefinitions.Remove(chart);
            return await _storageService.UpdateJobView(jobId, jobView);
        }

        private void EnsureCorrectViewConfiguration(JobView jobView)
        {
            if (jobView.ViewConfiguration == null)
            {
                jobView.ViewConfiguration = new ViewConfiguration {ChartDefinitions = new List<ChartDefinition>() };
            }

            if (jobView.ViewConfiguration.ChartDefinitions == null)
            {
                jobView.ViewConfiguration.ChartDefinitions = new List<ChartDefinition>();
            }
        }
    }
}