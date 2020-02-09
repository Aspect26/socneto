using System;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IGetAnalysisService
    {
        
        Task<AggregationAnalysisResult> GetAggregationAnalysis(Guid jobId, string analyserId, string analysisProperty);

        Task<ArrayAnalysisResult> GetArrayAnalysis(Guid jobId, string analyserId, string[] analysisProperties, bool isXPostDate);

    }
}