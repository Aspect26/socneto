using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IChartsService
    {
        Task<IList<ChartDefinition>> GetJobCharts(Guid jobId);

        Task<JobView> CreateJobChart(Guid jobId, string title, ChartType chartType, List<AnalysisDataPath> analysisDataPaths, bool isXPostDateTime);
    }
}