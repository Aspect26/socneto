using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Api.Models
{

    // TODO: to lowercase somehow
    public enum AnalysisType
    {
        AGGREGATION,
        LIST
    }

    public enum AnalysisResultType
    {
        MAP_SUM,
        LIST_SUM,
        LIST_WITH_TIME,
        LIST
    }

    public enum AnalysisResultValue
    {
        numberValue,
        textValue,
        numberListValue,
        testListValue,
        numberMapValue,
        textMapValue
    }
    
    public class GetAggregationAnalysisStorageRequest
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisResultType ResultRequestType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("resultName")]
        public string AnalysisProperty { get; set; }
        
        [JsonProperty("valueName")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisResultValue AnalysisResultValue { get; set; }
    }
    
    public class GetArrayAnalysisStorageRequest
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisResultType ResultRequestType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("params")]
        public List<ArrayAnalysisRequestProperty> AnalysisProperties { get; set; }
    }

    public class ArrayAnalysisRequestProperty
    {
        [JsonProperty("resultName")]
        public string AnalysisProperty { get; set; }
        
        [JsonProperty("valueName")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisResultValue AnalysisResultValue { get; set; }
    }
}