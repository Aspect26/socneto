using System.Linq;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class GetAnalysisService : IGetAnalysisService
    {
        
        private readonly IStorageService _storageService;

        public GetAnalysisService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public async Task<AggregationAnalysisResult> GetAggregationAnalysis(string analyserId, string analysisProperty)
        {
            var storageRequest = new GetAggregationAnalysisStorageRequest
            {
                Type = AnalysisType.AGGREGATION,
                ResultRequestType = AnalysisResultType.MAP_SUM,           // TODO: this should be computed from the analyser's data format
                ComponentId = analyserId,
                AnalysisProperty = analysisProperty,
                AnalysisResultValue = AnalysisResultValue.numberMapValue  // TODO: this should be computed from the analyser's data format
            };
            
            return await _storageService.GetAnalysisAggregation(storageRequest);
        }

        public async Task<ArrayAnalysisResult> GetArrayAnalysis(string analyserId, string[] analysisProperties)
        {
            var arrayAnalysisStorageRequest = new GetArrayAnalysisStorageRequest
            {
                Type = AnalysisType.LIST,
                ResultRequestType = AnalysisResultType.LIST_WITH_TIME,     // TODO: this should be computed from the analyser's data format
                ComponentId = analyserId,
                AnalysisProperties = analysisProperties.Select(analysisProperty => new ArrayAnalysisRequestProperty
                {
                    AnalysisProperty = analysisProperty,
                    AnalysisResultValue = AnalysisResultValue.numberValue  // TODO: this should be computed from the analyser's data format
                }).ToList()
            };
            
            return await _storageService.GetAnalysisArray(arrayAnalysisStorageRequest);
        }
        
    }
}