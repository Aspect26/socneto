using System.ComponentModel.DataAnnotations;

namespace ConsoleApi.Reddit
{
    public class RedditCredentialsOptions
    {
        [Required]
        public string AppId { get; set; }
        [Required] 
        public string AppSecret { get; set; }
        [Required] 
        public string RefreshToken { get; set; }
    }
}
