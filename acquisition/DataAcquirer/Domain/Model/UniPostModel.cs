using System;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class DataAcquirerPost
    {
        private DataAcquirerPost(
         string postId,
         string text,
         string source,
         string userId,
         string postDateTime)
        {
            PostId = postId;
            Text = text;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
        }

        public string PostId { get; }

        public string Text { get; }

        public string Source { get; }

        public string UserId { get; }

        public string PostDateTime { get; }

        public static DataAcquirerPost FromValues(
            string postId,
            string text,
            string source,
            string userId,
            string dateTimeString)
        {
            return new DataAcquirerPost(postId, text, source, userId, dateTimeString);
        }

    }


    public class UniPostModel
    {
        private UniPostModel(
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

        public static UniPostModel FromValues(
            string postId,
            string text,
            string source,
            string userId,
            string dateTimeString,
            Guid jobId)
        {
            return new UniPostModel(postId, text, source, userId, dateTimeString, jobId);
        }


    }
}
