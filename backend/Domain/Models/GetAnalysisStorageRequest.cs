using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models
{
    public class GetAggregationAnalysisStorageRequest
    {
        [JsonProperty("type")]
        public AnalysisType Type { get; set; }
        
        [JsonProperty("resultRequestType")]
        public AnalysisResultType ResultRequestType { get; set; }
        
        [JsonProperty("componentId")]
        public string ComponentId { get; set; }
        
        [JsonProperty("resultName")]
        public string AnalysisProperty { get; set; }
        
        [JsonProperty("valueName")]
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
        
        [EnumMember(Value = "LIST_SUM")]
        ListSum,
        
        [EnumMember(Value = "LIST_WITH_TIME")]
        ListWithTime,
        
        [EnumMember(Value = "LIST")]
        List
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisResultValue
    {
        [EnumMember(Value = "numberValue")]
        NumberValue,
        
        [EnumMember(Value = "textValue")]
        TextValue,
        
        [EnumMember(Value = "numberListValue")]
        NumberListValue,
        
        [EnumMember(Value = "testListValue")]
        TestListValue,
        
        [EnumMember(Value = "numberMapValue")]
        NumberMapValue,
        
        [EnumMember(Value = "textMapValue")]
        TextMapValue
    }
}