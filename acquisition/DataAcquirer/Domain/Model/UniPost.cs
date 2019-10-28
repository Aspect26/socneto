using System;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Model
{
    
    public class UniPost
    {
        private UniPost(
            string postId,
            string text,
            string source,
            string userId,
            string postDateTime,
            Guid jobId)
        {
            PostId = postId;
            Text = text;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
            JobId = jobId;
        }

        [JsonProperty("postId")]
        public string PostId { get; }

        [JsonProperty("jobId")]
        public Guid JobId { get; }

        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("authorId")]
        public string UserId { get; }

        [JsonProperty("dateTime")]
        public string PostDateTime { get; }

        public static UniPost FromValues(
            string postId,
            string text,
            string source,
            string userId,
            string dateTimeString,
            Guid jobId)
        {
            return new UniPost(postId, text, source, userId, dateTimeString, jobId);
        }


    }
}
