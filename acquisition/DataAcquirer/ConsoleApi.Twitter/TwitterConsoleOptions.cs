using System.ComponentModel.DataAnnotations;

namespace ConsoleApi.Twitter
{
    public class TwitterConsoleOptions
        {
            [Required]
            public string Query { get; set; }
        }
}
