namespace Socneto.Domain
{
    public class DefaultAcquirersCredentialsOptions
    {
        public DefaultTwitterCredentialsOptions Twitter { get; set; }
        public DefaultRedditCredentialsOptions Reddit { get; set; }
    }

    public class DefaultTwitterCredentialsOptions
    {
        public string ApiKey { get; set; }
        
        public string ApiSecretKey { get; set; }
        
        public string AccessToken { get; set; }
        
        public string AccessTokenSecret { get; set; }
    }
    
    public class DefaultRedditCredentialsOptions 
    {
        public string AppId { get; set; }
        
        public string AppSecret { get; set; }
        
        public string RefreshToken { get; set; }
    }
}