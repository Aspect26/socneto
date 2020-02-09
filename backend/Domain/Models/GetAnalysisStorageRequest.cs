using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models
{
    public class GetAggregationAnalysisStorageRequest
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("type")]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        public AnalysisResultType ResultType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("resultName")]
        public string AnalysisPropertyName { get; set; }
        
        [JsonProperty("valueName")]
        public AnalysisPropertyType AnalysisPropertyType { get; set; }
    }

    public class GetArrayAnalysisStorageRequest
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("type")]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        public AnalysisResultType ResultType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("params")]
        public List<ArrayAnalysisRequestProperty> AnalysisProperties { get; set; }
    }

    public class ArrayAnalysisRequestProperty
    {
        [JsonProperty("resultName")]
        public string AnalysisPropertyName { get; set; }
        
        [JsonProperty("valueName")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisPropertyType AnalysisPropertyType { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisType
    {
        [EnumMember(Value = "AGGREGATION")]
        Aggregation,
        
        [EnumMember(Value = "LIST")]
        List,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisResultType
    {
        [EnumMember(Value = "MAP_SUM")]
        MapSum,
        
        [EnumMember(Value = "LIST_COUNT")]
        ListCount,
        
        [EnumMember(Value = "LIST_WITH_TIME")]
        ListWithTime,
        
        [EnumMember(Value = "LIST")]
        List
    }
}
