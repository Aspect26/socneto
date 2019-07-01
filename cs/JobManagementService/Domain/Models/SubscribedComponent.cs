namespace Domain.Models
{
    public class SubscribedComponent
    {
        public string ComponentId { get; }
        public string ComponentType { get; }
        public string ChannelName { get; }

        public SubscribedComponent(
            string componentId,
            string componentType, 
            string channelName)
        {
            ComponentId = componentId;
            ComponentType = componentType;
            ChannelName = channelName;
        }
    }
}