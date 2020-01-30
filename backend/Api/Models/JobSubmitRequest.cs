using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class JobSubmitRequest
    {
        [JsonProperty("job_name")]
        public string JobName { get; set; }
        
        [JsonProperty("topic_query")]
        public string TopicQuery { get; set; }
        
        [JsonProperty("selected_acquirers")]
        public string[] SelectedAcquirers { get; set; }
        
        [JsonProperty("selected_analysers")]
        public string[] SelectedAnalysers { get; set; }
        
        // TODO: here we use model from Domain no API :(
        [JsonProperty("language")]
        public Language Language { get; set; } 
        
        [JsonProperty("credentials")]
        public Dictionary<string, AcquirerCredentials> Credentials { get; set; }
    }

    public class AcquirerCredentials
    {
        [JsonProperty("twitter")]
        public TwitterCredentials TwitterCredentials { get; set; }
        
        [JsonProperty("reddit")]
        public RedditCredentials RedditCredentials { get; set; }
    }

    public class TwitterCredentials
    {
        [JsonProperty("api_key")] 
        public string ApiKey { get; set; }

        [JsonProperty("api_secret_key")] 
        public string ApiSecretKey { get; set; }
        
        [JsonProperty("access_token")] 
        public string AccessToken { get; set; }
        
        [JsonProperty("access_token_secret")] 
        public string AccessTokenSecret { get; set; }
    }

    public class RedditCredentials
    {
        [JsonProperty("app_id")] 
        public string AppId { get; set; }
        
        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}