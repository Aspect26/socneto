namespace Infrastructure.Twitter
{
    public class TwitterCredentials
    {
        public string ConsumerKey { get; internal set; }
        public string ConsumerSecret { get; internal set; }
        public string AccessToken { get; internal set; }
        public string AccessTokenSecret { get; internal set; }
    }
}
