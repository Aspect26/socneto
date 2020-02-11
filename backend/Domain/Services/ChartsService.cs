using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

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
            return jobView?.ViewConfiguration == null ? new List<ChartDefinition>() : jobView.ViewConfiguration.ChartDefinitions;
        }

        public async Task<JobView> CreateJobChart(Guid jobId, string title, ChartType chartType, List<AnalysisDataPath> analysisDataPaths, bool isXPostDateTime)
        {
            var newChartDefinition = new ChartDefinition
            {
                Title = title,
                ChartType = chartType,
                AnalysisDataPaths = analysisDataPaths,
                IsXPostDatetime = isXPostDateTime
            };

            var jobView = await _storageService.GetJobView(jobId);
            EnsureCorrectViewConfiguration(jobView);
            jobView.ViewConfiguration.ChartDefinitions.Add(newChartDefinition);

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