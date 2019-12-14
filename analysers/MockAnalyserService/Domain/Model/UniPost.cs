using System;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class UniPost
    {
        public UniPost()
        {
            
        }
        private UniPost(
            Guid postId, 
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
        public Guid PostId { set; get; }
        
        [JsonProperty("originalPostId")]
        public string OriginalPostId { get; set; }

        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("authorId")]
        public string UserId { get; set; }

        [JsonProperty("dateTime")]
        public string PostDateTime { get; set; }

        public static UniPost FromValues(
            Guid postId, 
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
