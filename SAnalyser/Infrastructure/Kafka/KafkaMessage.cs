using Confluent.Kafka;
using Domain.Models;

namespace Infrastructure
{
    public class KafkaMessage: Message<string,string>
    {
        public static KafkaMessage ToKafka(Message message)
        {
            return new KafkaMessage() { Key= message.Key, Value = message.Value};
        }
    }

}
