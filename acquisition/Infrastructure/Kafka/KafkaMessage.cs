using Confluent.Kafka;
using Domain;

namespace Infrastructure.Kafka
{
    public class KafkaMessage : Message<string, string>
    {
        public static KafkaMessage FromMessageBrokerMessage
            (MessageBrokerMessage registrationRequest)
        {
            return new KafkaMessage()
            {
                Key = registrationRequest.Key,
                Value = registrationRequest.JsonPayloadPayload
            };
        }
    }

}
