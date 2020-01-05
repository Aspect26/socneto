using System;

namespace Infrastructure.Twitter
{
    public class TwitterBatchLoaderOptions
    {
        public TimeSpan NoPostWaitDelay { get; set; }
        public TimeSpan RateLimitExceededWaitDelay { get;  set; }
        public TimeSpan ErrorEncounteredWaitDelay { get; set; }
    }
}
