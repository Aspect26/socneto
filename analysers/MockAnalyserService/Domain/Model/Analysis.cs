using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.Model
{
    public class Analysis
    {
        public Dictionary<string, AnalysisValue> Data { get; set; }
    }

    public class AnalysisValue
    {
        // TODO: omg dynamic
        public dynamic Value { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisValueType ValueType { get; set; }
        
    }

    public enum AnalysisValueType
    {
        Number, String, NumberList, StringList
    }
}