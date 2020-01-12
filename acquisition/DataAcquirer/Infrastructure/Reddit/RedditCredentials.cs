namespace Infrastructure.Reddit
{
    public class RedditCredentials
    {
        public RedditCredentials(
            string appId,
            string apiSecret,
            string refreshToken)
        {
            AppId = appId;
            ApiSecret = apiSecret;
            RefreshToken = refreshToken;
        }

        public string AppId { get; }
        public string ApiSecret { get; }
        public string RefreshToken { get; }
    }

}
