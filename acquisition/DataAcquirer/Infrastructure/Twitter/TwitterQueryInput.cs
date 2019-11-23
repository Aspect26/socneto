namespace Infrastructure.Twitter
{
    public class TwitterQueryInput
    {
        public TwitterQueryInput(
            string searchTerm,
            string language,
            ulong maxId,
            ulong sinceId,
            int batchSize)
        {
            SinceId = sinceId;
            BatchSize = batchSize;
            SearchTerm = searchTerm;
            Language = language;
            MaxId = maxId;
        }

        public string SearchTerm { get; }
        public string Language { get; }
        public ulong MaxId { get; }
        public ulong SinceId { get; }
        public int BatchSize { get; }
    }
}
