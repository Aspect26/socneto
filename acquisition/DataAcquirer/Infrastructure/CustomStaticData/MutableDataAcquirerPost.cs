using System;
using Domain.Model;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{
    public class MutableDataAcquirerPost
    {
        [JsonProperty("originalPostId")]
        public string OriginalPostId { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("authorId")]
        public string UserId { get; set; }

        [JsonProperty("dateTime")]
        public string DateTime { get; set; }

        public DataAcquirerPost Freeze()
        {
            var datetime = DateTime ?? System.DateTime.Now.ToString("s");
            return DataAcquirerPost.FromValues(
                            OriginalPostId,
                            Text,
                            Language,
                            Source,
                            UserId,
                            datetime,
                            Query);

        }
    }
}
