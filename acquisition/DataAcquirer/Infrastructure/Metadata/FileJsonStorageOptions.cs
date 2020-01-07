using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Metadata
{
    public class FileJsonStorageOptions
    {
        [Required]
        public string Directory { get; set; }
        
        [Required]
        public string FilePathTemplate { get; set; }
    }
}
