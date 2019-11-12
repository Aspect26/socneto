namespace Infrastructure.Twitter
{
    public class TwitterQueryInput
    {
        public TwitterQueryInput(
            string searchTerm,
            ulong maxId,
            ulong sinceId,
            int batchSize)
        {
            SinceId = sinceId;
            BatchSize = batchSize;
            SearchTerm = searchTerm;
            MaxId = maxId;
        }

        public string SearchTerm { get; }
        public ulong MaxId { get; }
        public ulong SinceId { get; }
        public int BatchSize { get; }
    }
}
