namespace Domain
{
    public class MessageBrokerMessage
    {
        
        public MessageBrokerMessage(string messageKey, string jsonPayload)
        {
            Key = messageKey;
            JsonPayloadPayload = jsonPayload;
        }

        public string Key { get; }
        public string JsonPayloadPayload { get; }
    }
}