namespace Socneto.Domain.Models
{
    public class UniPost
    {
        private UniPost(string text, string source, int userId, string dateTime)
        {
            Text = text;
            UserIdId = userId;
            Source= source;
            DateTime = dateTime;
        }
        public string Text { get; set; }
        public string Source { get; set; }
        public int UserIdId { get; set; }

        public string DateTime { get; set; }

        public static UniPost FromValues(string text, string source, int userId, string dateTimeString)
        {
            return new UniPost(text, source, userId, dateTimeString);
        }

    }
}
