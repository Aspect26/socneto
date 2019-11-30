using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Models
{
    public class ArrayAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        // TODO: object? :(
        [JsonProperty("list")]
        public List<List<object>> Result { get; set; }
    }
}