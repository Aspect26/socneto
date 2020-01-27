using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Socneto.Domain.Models
{
    public class ArrayAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        [JsonProperty("list")]
        public List<JArray> Result { get; set; }
    }
}