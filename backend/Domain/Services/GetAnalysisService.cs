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
                AnalysisPropertyName = analysisProperty,
                AnalysisPropertyType = analysisPropertyType
            };
            
            return await _storageService.GetAnalysisAggregation(storageRequest);
        }

        public async Task<ArrayAnalysisResult> GetArrayAnalysis(Guid jobId, string analyserId, string[] analysisProperties, bool isXPostDate)
        {
            return isXPostDate
                ? await GetListWithTimeAnalysis(jobId, analyserId, analysisProperties) 
                : await GetListAnalysis(jobId, analyserId, analysisProperties);  
        }

        private async Task<ArrayAnalysisResult> GetListWithTimeAnalysis(Guid jobId, string analyserId, string[] analysisProperties)
        {
            List<TimeArrayAnalysisResult> analyses = new List<TimeArrayAnalysisResult>();
            
            foreach (var analysisProperty in analysisProperties)
            {
                var analysisPropertyRequest = new ArrayAnalysisRequestProperty
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
                    AnalysisProperties = new List<ArrayAnalysisRequestProperty> { analysisPropertyRequest } 
                };
                
                var analysis = await _storageService.GetAnalysisTimeArray(arrayAnalysisStorageRequest);
                analyses.Add(analysis);
            }

            return MergeTimeAnalyses(analyses);
        }

        private ArrayAnalysisResult MergeTimeAnalyses(List<TimeArrayAnalysisResult> analyses)
        {
            if (analyses.Count == 0) return new ArrayAnalysisResult();

            var result = new ArrayAnalysisResult
            {
                ResultName = analyses[0].ResultName,
                Result = analyses[0].Result.Select(dataTuple => new JArray(dataTuple)).ToList()
            };

            foreach (var analysisResult in analyses.GetRange(1, analyses.Count - 1))
            {
                ConcatTimeAnalyses(result, analysisResult);
            }

            return result;
        }

        private void ConcatTimeAnalyses(ArrayAnalysisResult to, TimeArrayAnalysisResult from)
        {
            int toIndex;
            int fromIndex;

            
            for (toIndex = 0, fromIndex = 0; toIndex < to.Result.Count && fromIndex < from.Result.Count;)
            {
                var dateTimesDiff = DateTime.Compare(DateTime.Parse((string)to.Result[toIndex][0]), from.Result[fromIndex].Item1);
                if (toIndex == to.Result.Count)
                {
                    to.Result.AddRange(from.Result.GetRange(fromIndex, from.Result.Count - fromIndex)
                        .Select(dataTuple => new JArray(dataTuple.Item1, dataTuple.Item2)));
                }
                else if (fromIndex == from.Result.Count)
                {
                    return;
                }
                else if (dateTimesDiff == 0)
                {
                    to.Result[toIndex].Add(from.Result[fromIndex].Item2);
                    fromIndex++;
                    toIndex++;
                } 
                else if (dateTimesDiff > 0)
                {
                    to.Result.Insert(toIndex, new JArray(from.Result[fromIndex].Item1, from.Result[fromIndex].Item2));
                    fromIndex++;
                }
                else
                {
                    toIndex++;
                }
            }
        }

        private async Task<ArrayAnalysisResult> GetListAnalysis(Guid jobId, string analyserId, string[] analysisProperties)
        {
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
                JobId = jobId,
                Type = AnalysisType.List,
                ResultType = AnalysisResultType.List,
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