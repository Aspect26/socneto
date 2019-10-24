using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Kafka
{
    public class KafkaOptions
    {
        [Required]
        public string ServerAddress { get; set; }

    }

}
