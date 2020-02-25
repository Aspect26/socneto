using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Api.Models.Responses
{
    public class Paginated<T>
    {
        [JsonProperty("data")]
        public IList<T> Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("total_size")] 
        public int TotalSize { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
    }
}