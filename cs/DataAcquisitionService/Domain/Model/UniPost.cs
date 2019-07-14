using System;
using System.Text;

namespace Domain.Model
{
    public class UniPost
    {
        private UniPost(string text, string source, string userId, string dateTime)
        {
            Text = text;
            UserId = userId;
            Source = source;
            DateTime = dateTime;
        }
        public string Text { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }

        public string DateTime { get; set; }

        public static UniPost FromValues(string text, string source, string userId, string dateTimeString)
        {
            return new UniPost(text, source, userId, dateTimeString);
        }

    }
}
