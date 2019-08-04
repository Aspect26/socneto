using System;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Model
{

    //public class UniPost
    //{
    //    public UniPost() { }

    //    private UniPost(
    //        string postId,
    //        string text,
    //        string source,
    //        string userId,
    //        string postDateTime,
    //        Guid jobId)
    //    {
    //        PostId = postId;
    //        Text = text;
    //        UserId = userId;
    //        Source = source;
    //        PostDateTime = postDateTime;
    //        JobId = jobId;
    //    }

    //    [JsonProperty("postId")]
    //    public string PostId { set; get; }

    //    [JsonProperty("jobId")]
    //    public Guid JobId { get; set; }

    //    [JsonProperty("text")]
    //    public string Text { get; set; }

    //    [JsonProperty("source")]
    //    public string Source { get; set; }

    //    [JsonProperty("userId")]
    //    public string UserId { get; set; }

    //    [JsonProperty("postDateTime")]
    //    public string PostDateTime { get; set; }

    //    public static UniPost FromValues(
    //        string postId,
    //        string text,
    //        string source,
    //        string userId,
    //        string dateTimeString,
    //        Guid jobId)
    //    {
    //        return new UniPost(postId, text, source, userId, dateTimeString, jobId);
    //    }
        public class UniPost
    {
        private UniPost(string text, string source, string userId, string postDateTime)
        {
            Text = text;
            UserId = userId;
            Source = source;
            PostDateTime = postDateTime;
        }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("postDateTime")]
        public string PostDateTime { get; set; }

        public static UniPost FromValues(string text, string source, string userId, string dateTimeString)
        {
            return new UniPost(text, source, userId, dateTimeString);
        }

    }
}
