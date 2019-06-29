namespace Domain.Models
{
    public class ComponentRegisterModel
    {
        public string JobConfigUpdaterChannelName { get; }
        public string ComponentType { get; }
        
        public ComponentRegisterModel(string channelName, string requestComponentType)
        {
            JobConfigUpdaterChannelName = channelName;
            ComponentType = requestComponentType;
        }
    }
}