using Domain.Acquisition;

namespace Infrastructure.Twitter
{
    public class TwitterMetadata : IDataAcquirerMetadata
    {
        public TwitterCredentials Credentials { get; set; }
        public ulong MaxId { get;  set; }
        public ulong SinceId { get; set; }
        public string Query { get; set; }
        public string Language { get; set; }
        public int BatchSize { get; set; }
    }
}
