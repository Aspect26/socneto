namespace Domain.Models
{
    public class ComponentRegistrationModel
    {
        public string ComponentId { get; }
        public string ChannelId { get; }
        public string ComponentType { get; }
        
        public ComponentRegistrationModel(string componentId, string channelId, string requestComponentType)
        {
            ComponentId = componentId;
            ChannelId = channelId;
            ComponentType = requestComponentType;
        }
    }
}