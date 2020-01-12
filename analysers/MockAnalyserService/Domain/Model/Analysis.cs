using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class Analysis
    {
        public Dictionary<string, AnalysisResult> Data { get; set; }
    }

    public class AnalysisResult
    {
        [JsonProperty("numberValue")]
        public double NumberValue { get; set; }
        
        [JsonProperty("textValue")]
        public string TextValue { get; set; }
        
        [JsonProperty("numberListValue")]
        public double[] NumberListValue { get; set; }
        
        [JsonProperty("textListValue")]
        public string[] TextListValue { get; set; }
        
        [JsonProperty("numberMapValue")]
        public Dictionary<string, double> NumberMapValue { get; set; }
        
        [JsonProperty("textMapValue")]
        public Dictionary<string, string> StringMapValue { get; set; }
    }
}