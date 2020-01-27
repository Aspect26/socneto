using System;
using System.Collections.Generic;
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
            var analysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperty);
            var resultType =
                analysisPropertyType == AnalysisPropertyType.NumberMap ||
                analysisPropertyType == AnalysisPropertyType.StringMap
                    ? AnalysisResultType.MapSum
                    : AnalysisResultType.ListCount;
            
            var storageRequest = new GetAggregationAnalysisStorageRequest
            {
                Type = AnalysisType.Aggregation,
                ResultType = resultType,
                ComponentId = analyserId,
                AnalysisPropertyName = analysisProperty,
                AnalysisPropertyType = analysisPropertyType
            };
            
            return await _storageService.GetAnalysisAggregation(storageRequest);
        }

        public async Task<ArrayAnalysisResult> GetArrayAnalysis(string analyserId, string[] analysisProperties, bool isXPostDate)
        {
            var analysisResultType = isXPostDate ? AnalysisResultType.ListWithTime : AnalysisResultType.List;
            var analysisPropertiesRequest = new List<ArrayAnalysisRequestProperty>();
            
            foreach (var analysisProperty in analysisProperties)
            {
                analysisPropertiesRequest.Add(new ArrayAnalysisRequestProperty
                {
                    AnalysisPropertyName = analysisProperty,
                    AnalysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperty)
                });
            }
            
            var arrayAnalysisStorageRequest = new GetArrayAnalysisStorageRequest
            {
                Type = AnalysisType.List,
                ResultType = analysisResultType,
                ComponentId = analyserId,
                AnalysisProperties = analysisPropertiesRequest
            };
            
            return await _storageService.GetAnalysisArray(arrayAnalysisStorageRequest);
        }

        private async Task<AnalysisPropertyType> GetAnalyserPropertyResultValue(string analyserId, string analyserProperty)
        {
            var analyser = await _storageService.GetAnalyser(analyserId);
            if (analyser == null)
            {
                throw new GetAnalysisException(analyserId, analyserProperty, "The analyser is not connected to the platform");
            }

            var analyserOutputFormat = analyser.Attributes?["outputFormat"]?.ToObject<Dictionary<string, AnalysisPropertyType>>();
            if (analyserOutputFormat == null || analyserOutputFormat.Keys.Count == 0)
            {
                throw new GetAnalysisException(analyserId, analyserProperty, "Output format of the analyser does not contain any properties");
            }

            if (!analyserOutputFormat.ContainsKey(analyserProperty))
            {
                throw new GetAnalysisException(analyserId, analyserProperty, "The analyser does not contain given property");
            }

            return analyserOutputFormat[analyserProperty];
        }

        public class GetAnalysisException : Exception
        {
            public GetAnalysisException(string analyserId, string analysisProperty, string message, Exception inner)
                : base(GetErrorMessage(analyserId, analysisProperty, message), inner)
            {
            }
            
            public GetAnalysisException(string analyserId, string analysisProperty, string message)
                : base(GetErrorMessage(analyserId, analysisProperty, message))
            {
            }

            private static string GetErrorMessage(string analyserId, string analysisProperty, string message) =>
                $"Error retrieving analyses from analyser {analyserId}, for property {analysisProperty}: {message}";
        }
        
    }
}