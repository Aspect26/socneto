using Confluent.Kafka;
using Socneto.DataAcquisition.Domain;
using Socneto.DataAcquisition.Domain;

namespace Socneto.DataAcquisition.Infrastructure.Kafka
{
    public class KafkaMessage: Message<string,string>
    {

        public static KafkaMessage ToKafka(Message message)
        {
            return new KafkaMessage() { Key= message.Key, Value = message.Value};
        }
    }

}
