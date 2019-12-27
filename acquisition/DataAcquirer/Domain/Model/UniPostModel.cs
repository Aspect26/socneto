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
         string language,
         string source,
         string userId,
         string postDateTime,
         string query)
        {
            PostId = postId;
            Text = text;
            Language = language;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
            Query = query;
        }

        public string PostId { get; }

        public string Text { get; }

        public string Source { get; }

        public string UserId { get; }

        public string PostDateTime { get; }
        public string Query { get; }
        public string Language { get; }

        public static DataAcquirerPost FromValues(
            string postId,
            string text,
            string language,
            string source,
            string userId,
            string dateTimeString,
            string query = null)
        {
            return new DataAcquirerPost(postId, text,language, source, userId, dateTimeString,query);
        }
    }


    public class UniPostModel
    {
        private UniPostModel(
            string postId,
            string text,
            string language,
            string source,
            string userId,
            string postDateTime,
            Guid jobId,
            string query)
        {
            PostId = postId;
            Text = text;
            Language = language;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
            JobId = jobId;
            Query = query;
        }

        [JsonProperty("postId")]
        public string PostId { get; }

        [JsonProperty("jobId")]
        public Guid JobId { get; }

        [JsonProperty("query")]
        public string Query { get; }
        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("language")]
        public string Language { get; }
        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("authorId")]
        public string UserId { get; }

        [JsonProperty("dateTime")]
        public string PostDateTime { get; }

        public static UniPostModel FromValues(
            string postId,
            string text,
            string language,
            string source,
            string userId,
            string dateTimeString,
            Guid jobId,
            string query)
        {

            return new UniPostModel(postId, text,language, source, userId, dateTimeString, jobId,query);
        }


    }
}
