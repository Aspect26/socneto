using Confluent.Kafka;
using Socneto.Domain.Models;

namespace Socneto.Infrastructure.Kafka
{
    public class KafkaMessage: Message<string,string>
    {

        public static KafkaMessage ToKafka(Message message)
        {
            return new KafkaMessage() { Key= message.Key, Value = message.Value};
        }
    }

}
