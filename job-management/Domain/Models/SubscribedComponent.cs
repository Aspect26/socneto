namespace Domain.Models
{
    public class SubscribedComponent
    {
        public string ComponentId { get; }
        public string ComponentType { get; }
        public string InputChannelName { get; }
        public string UpdateChannelName { get; }

        public SubscribedComponent(
            string componentId,
            string componentType,
            string inputChannelName,
            string updateChannelName)
        {
            ComponentId = componentId;
            ComponentType = componentType;
            InputChannelName = inputChannelName;
            UpdateChannelName = updateChannelName;
        }
    }
}