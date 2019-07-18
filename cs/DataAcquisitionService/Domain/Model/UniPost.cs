using System;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Model
{
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
