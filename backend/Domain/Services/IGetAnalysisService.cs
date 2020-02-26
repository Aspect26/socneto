using System;
using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Domain.Services
{
    public interface IGetAnalysisService
    {
        Task<AggregationAnalysisResult> GetPostsFrequency(Guid jobId);
        
        Task<AggregationAnalysisResult> GetLanguageFrequency(Guid jobId);
        
        Task<AggregationAnalysisResult> GetAuthorFrequency(Guid jobId);
        
        Task<AggregationAnalysisResult> GetAggregationAnalysis(Guid jobId, string analyserId, string analysisProperty);

        Task<ArrayAnalysisResult> GetArrayAnalysis(Guid jobId, string analyserId, string[] analysisProperties, 
            bool isXPostDate, int resultSize, int page);

    }
}