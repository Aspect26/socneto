namespace Infrastructure.StaticData
{
    public class UniPostStaticData
    {
        public UniPostStaticData(
            string postId,
            string text,
            string language,
            string source,
            string userId,
            string postDateTime)
        {
            PostId = postId;
            Text = text;
            Language = language;
            Source = source;
            UserId = userId;
            PostDateTime = postDateTime;
        }

        public string Source { get; }

        public string PostId { get; }

        public string Text { get; }
        public string Language { get; }
        public string UserId { get; }

        public string PostDateTime { get; }
    }

}
