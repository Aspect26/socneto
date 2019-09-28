using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models
{
    public class AnalysisValue
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalysisValueType ValueType { get; set; }
        
        public double Value { get; set; }
    }

    public enum AnalysisValueType
    {
        Number,
        String,
    }
}