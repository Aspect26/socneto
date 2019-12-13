using System.ComponentModel.DataAnnotations;

namespace Kafka
{
    public class KafkaOptions
    {
        [Required]
        public string ServerAddress { get; set; }

    }

}
