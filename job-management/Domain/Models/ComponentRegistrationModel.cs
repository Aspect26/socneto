namespace Domain.Models
{
    public class ComponentRegistrationModel
    {
        public string ComponentId { get; }
        public string UpdateChannelId { get; }
        public string ComponentType { get; }
        public string InputChannelId { get; }
        
        public ComponentRegistrationModel(
            string componentId, 
            string updateChannelId, 
            string inputChannelId,
            string requestComponentType)
        {
            ComponentId = componentId;
            UpdateChannelId = updateChannelId;
            InputChannelId = inputChannelId;
            ComponentType = requestComponentType;
        }

    }
}