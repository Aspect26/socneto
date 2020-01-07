using System;

namespace Infrastructure.StaticData
{
    public class UniPostStaticData
    {
        public UniPostStaticData(
            Guid postId,
            string originalPostId,
            string text,
            string language,
            string source,
            string userId,
            string postDateTime)
        {
            PostId = postId;
            OriginalPostId = originalPostId;
            Text = text;
            Language = language;
            Source = source;
            UserId = userId;
            PostDateTime = postDateTime;
        }

        public string Source { get; }

        public Guid PostId { get; }

        public string OriginalPostId { get;}

        public string Text { get; }
        public string Language { get; }
        public string UserId { get; }

        public string PostDateTime { get; }
        
    }

}
