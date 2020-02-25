using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Domain.Services
{
    public interface IChartsService
    {
        Task<IList<ChartDefinition>> GetJobCharts(Guid jobId);

        Task<ChartDefinition> CreateJobChart(Guid jobId, string title, ChartType chartType, List<AnalysisDataPath> analysisDataPaths, bool isXPostDateTime);

        Task<JobView> RemoveJobChart(Guid jobId, Guid chartId);
    }
}