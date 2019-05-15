using Confluent.Kafka;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Infrastructure.Kafka
{
    public class KafkaMessage: Message<string,string>
    {
        public static KafkaMessage ToKafka(Message message)
        {
            return new KafkaMessage() { Key= message.Key, Value = message.Value};
        }
    }

}
