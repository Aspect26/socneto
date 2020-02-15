using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models
{
    // TODO: check if all these classes cannot be merged together
    public class GetAggregationAnalysisStorageRequest
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        // TODO: readonly maybe, since it can be only AGGREGATION
        [JsonProperty("type")]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        public AnalysisResultType ResultType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("params")]
        public List<AnalysisRequestProperty> Properties { get; set; }
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
        
        // TODO: these can be only two, so maybe use Tuple
        [JsonProperty("params")]
        public List<AnalysisRequestProperty> AnalysisProperties { get; set; }

        [JsonProperty("size")] 
        public int ResultSize { get; set; }

        [JsonProperty("page")] 
        public int ResultPage { get; set; }
    }

    public class AnalysisRequestProperty
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
