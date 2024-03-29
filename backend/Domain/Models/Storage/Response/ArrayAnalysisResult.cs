using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socneto.Domain.Helpers;

namespace Socneto.Domain.Models.Storage.Response
{
    public class ArrayAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        [JsonProperty("list")]
        public List<JArray> Result { get; set; }
        
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }

    public class TimeArrayAnalysisResult
    {
        [JsonProperty("resultName")]
        public string ResultName { get; set; }
        
        [JsonProperty("list")]
        [JsonConverter(typeof(TupleListConverter<DateTime, JToken>))]
        public List<Tuple<DateTime, JToken>> Result { get; set; }
        
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
