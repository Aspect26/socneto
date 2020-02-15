using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class GetAnalysisService : IGetAnalysisService
    {
        private const int MaxResultSize = 500;
        
        private readonly IStorageService _storageService;
        private readonly IEventTracker<GetAnalysisService> _eventTracker;

        private const string EventTrackerEventMessage = "GetAnalysis";

        public GetAnalysisService(IStorageService storageService, IEventTracker<GetAnalysisService> eventTracker)
        {
            _storageService = storageService;
            _eventTracker = eventTracker;
        }
        
        public async Task<AggregationAnalysisResult> GetAggregationAnalysis(Guid jobId, string analyserId, string analysisProperty)
        {
            var analysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperty);
            var resultType =
                analysisPropertyType == AnalysisPropertyType.NumberMap ||
                analysisPropertyType == AnalysisPropertyType.StringMap
                    ? AnalysisResultType.MapSum
                    : AnalysisResultType.ListCount;
            
            var storageRequest = new GetAggregationAnalysisStorageRequest
            {
                JobId = jobId,
                Type = AnalysisType.Aggregation,
                ResultType = resultType,
                ComponentId = analyserId,
                Properties = new List<AnalysisRequestProperty>
                {
                    new AnalysisRequestProperty
                    {
                        AnalysisPropertyName = analysisProperty,
                        AnalysisPropertyType = analysisPropertyType
                    }
                }
            };
            
            return await _storageService.GetAnalysisAggregation(storageRequest);
        }

        public async Task<ArrayAnalysisResult> GetArrayAnalysis(Guid jobId, string analyserId, string[] analysisProperties, 
            bool isXPostDate, int resultSize, int page)
        {
            resultSize = Math.Min(resultSize, MaxResultSize);
            
            return isXPostDate
                ? await GetListWithTimeAnalysis(jobId, analyserId, analysisProperties, resultSize, page) 
                : await GetListAnalysis(jobId, analyserId, analysisProperties, resultSize, page);  
        }

        private async Task<ArrayAnalysisResult> GetListWithTimeAnalysis(Guid jobId, string analyserId, 
            string[] analysisProperties, int resultSize, int page)
        {
            if (analysisProperties.Length < 2) 
                throw new GetAnalysisException(analyserId, "", "List with time analysis request requires at least one analysis property");
            
            var analyses = new List<TimeArrayAnalysisResult>();
            
            foreach (var analysisProperty in analysisProperties)
            {
                var analysisPropertyRequest = new AnalysisRequestProperty
                {
                    AnalysisPropertyName = analysisProperty,
                    AnalysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperty)
                };
                
                var arrayAnalysisStorageRequest = new GetArrayAnalysisStorageRequest
                {
                    JobId = jobId,
                    Type = AnalysisType.List,
                    ResultType = AnalysisResultType.ListWithTime,
                    ComponentId = analyserId,
                    AnalysisProperties = new List<AnalysisRequestProperty> { analysisPropertyRequest },
                    ResultSize = resultSize,
                    ResultPage = page
                };
                
                var analysis = await _storageService.GetAnalysisTimeArray(arrayAnalysisStorageRequest);
                analyses.Add(analysis);
            }

            return MergeTimeAnalyses(analyses);
        }

        private ArrayAnalysisResult MergeTimeAnalyses(List<TimeArrayAnalysisResult> timeAnalyses)
        {
            var timeAnalysesResults = timeAnalyses.Select(timeAnalysis => timeAnalysis.Result);
            var result = timeAnalysesResults
                .Select(timeAnalysisResult => timeAnalysisResult.Select(dataPoint => new JArray(dataPoint.Item1, dataPoint.Item2)).ToList())
                .Select(timeAnalysisResultList => new JArray(timeAnalysisResultList)).ToList();

            return new ArrayAnalysisResult
            {
                ResultName = timeAnalyses[0].ResultName,
                Result = result
            };
        }

        private async Task<ArrayAnalysisResult> GetListAnalysis(Guid jobId, string analyserId, 
            string[] analysisProperties, int resultSize, int page)
        {
            if (analysisProperties.Length < 2) 
                throw new GetAnalysisException(analyserId, "", "List analysis request requires at least two analysis properties");
            
            var xAxisProperty = new AnalysisRequestProperty
            {
                AnalysisPropertyName = analysisProperties[0],
                AnalysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperties[0])
            };

            var analyses = new List<ArrayAnalysisResult>();
            
            for (var i = 1; i < analysisProperties.Length; ++i)
            {
                var currentAnalysisProperties = new List<AnalysisRequestProperty>
                {
                    xAxisProperty,
                    new AnalysisRequestProperty
                    {
                        AnalysisPropertyName = analysisProperties[i],
                        AnalysisPropertyType = await GetAnalyserPropertyResultValue(analyserId, analysisProperties[i])
                    }
                };
                
                var request = new GetArrayAnalysisStorageRequest
                {
                    JobId = jobId,
                    Type = AnalysisType.List,
                    ResultType = AnalysisResultType.List,
                    ComponentId = analyserId,
                    AnalysisProperties = currentAnalysisProperties,
                    ResultSize = resultSize,
                    ResultPage = page
                };
                
                var analysis = await _storageService.GetAnalysisArray(request);
                analyses.Add(analysis);
            }

            return MergeAnalyses(analyses);
        }
        
        private ArrayAnalysisResult MergeAnalyses(List<ArrayAnalysisResult> analyses)
        {
            var result = analyses
                .Select(analysis => analysis.Result).ToList()
                .Select(analysisResult => new JArray(analysisResult)).ToList();

            return new ArrayAnalysisResult
            {
                ResultName = analyses[0].ResultName,
                Result = result
            };
        }

        private async Task<AnalysisPropertyType> GetAnalyserPropertyResultValue(string analyserId, string analyserProperty)
        {
            var analyser = await _storageService.GetAnalyser(analyserId);
            if (analyser == null)
            {
                _eventTracker.TrackError(EventTrackerEventMessage, 
                    $"Cannot retrieve analysis because analyser '{analyserId}' is not connected to the platform");
                throw new GetAnalysisException(analyserId, analyserProperty, "The analyser is not connected to the platform");
            }

            var analyserOutputFormat = analyser.Attributes?["outputFormat"]?.ToObject<Dictionary<string, AnalysisPropertyType>>();
            if (analyserOutputFormat == null || analyserOutputFormat.Keys.Count == 0)
            {
                _eventTracker.TrackError(EventTrackerEventMessage, 
                    $"Cannot retrieve analysis because analyser '{analyserId}' doesn't contain any properties in its output format");
                throw new GetAnalysisException(analyserId, analyserProperty, "Output format of the analyser does not contain any properties");
            }

            if (analyserOutputFormat.ContainsKey(analyserProperty)) return analyserOutputFormat[analyserProperty];
            
            _eventTracker.TrackError(EventTrackerEventMessage, 
                $"Cannot retrieve analysis because analyser '{analyserId}' doesn't contain property {analyserProperty}");
            throw new GetAnalysisException(analyserId, analyserProperty, "The analyser does not contain given property");

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