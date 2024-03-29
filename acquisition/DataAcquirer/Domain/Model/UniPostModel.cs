using System;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class UniPostModel
    {
        private UniPostModel(
            Guid postId,
            string originalPostId,
            string text,
            string originalText,
            string language,
            string source,
            string userId,
            string postDateTime,
            Guid jobId,
            string query)
        {
            PostId = postId;
            OriginalPostId = originalPostId;
            Text = text;
            OriginalText = originalText;
            Language = language;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
            JobId = jobId;
            Query = query;
        }

        
        [JsonProperty("originalPostId")]
        public string OriginalPostId { get; }

        [JsonProperty("postId")]
        public Guid PostId { get; }
        
        [JsonProperty("jobId")]
        public Guid JobId { get; }

        [JsonProperty("query")]
        public string Query { get; }
        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("originalText")]
        public string OriginalText { get; }

        [JsonProperty("language")]
        public string Language { get; }
        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("authorId")]
        public string UserId { get; }

        [JsonProperty("dateTime")]
        public string PostDateTime { get; }

        public static UniPostModel FromValues(
            Guid postId,
            string originalPostId,
            string text,
            string originalText,
            string language,
            string source,
            string userId,
            string dateTimeString,
            Guid jobId,
            string query)
        {
            return new UniPostModel(postId,
                originalPostId,
                text,
                originalText,
                language,
                source,
                userId,
                dateTimeString,
                jobId,
                query);

        }

    }
}
