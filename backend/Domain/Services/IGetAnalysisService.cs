using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public interface IGetAnalysisService
    {
        
        Task<AggregationAnalysisResult> GetAggregationAnalysis(string analyserId, string analysisProperty);

        Task<ArrayAnalysisResult> GetArrayAnalysis(string analyserId, string[] analysisProperties, bool isXPostDate);

    }
}