namespace Domain.Model
{
    public class DataAcquirerPost
    {
        private DataAcquirerPost(
         string originalPostId,
         string text,
         string language,
         string source,
         string userId,
         string postDateTime,
         string query)
        {
            OriginalPostId = originalPostId;
            Text = text;
            Language = language;
            UserId = userId;
            Source = source;
            DateTime = postDateTime;
            Query = query;
        }

        public string OriginalPostId { get; }

        public string Text { get; }

        public string Source { get; }

        public string UserId { get; }

        public string DateTime { get; }
        public string Query { get; }
        public string Language { get; }

        public static DataAcquirerPost FromValues(
            string originalPostId,
            string text,
            string language,
            string source,
            string userId,
            string dateTimeString,
            string query = null)
        {
            return new DataAcquirerPost(originalPostId, text,language, source, userId, dateTimeString,query);
        }
    }
}
