using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Infrastructure.DataGenerator
{
    public class FileProducerOptions
    {
        [Required]
        public string DestinationFilePath { get; set; }
    }
}
