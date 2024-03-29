using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Models.Storage.Response
{
    public class ListWithCount<T>
    {
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("list")] 
        public List<T> Data { get; set; }
    }
}