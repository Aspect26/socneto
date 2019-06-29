namespace Domain.Models
{
    public class SubscribedComponent
    {
        public string ComponentType { get; }
        public string ChannelName { get; }

        public SubscribedComponent(string componentType, string channelName)
        {
            ComponentType = componentType;
            ChannelName = channelName;
        }
    }
}