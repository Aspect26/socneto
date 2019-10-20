using System.Collections.Generic;

namespace Domain.Models
{
    public class SubscribedComponent
    {
        public string ComponentId { get; }
        public string ComponentType { get; }
        public string InputChannelName { get; }
        public string UpdateChannelName { get; }

        public IReadOnlyDictionary<string, string> Attributes { get; }
        
        public SubscribedComponent(
            string componentId,
            string componentType,
            string inputChannelName,
            string updateChannelName,
            IReadOnlyDictionary<string,string> attributes)
        {
            ComponentId = componentId;
            ComponentType = componentType;
            InputChannelName = inputChannelName;
            UpdateChannelName = updateChannelName;
            Attributes = attributes;
        }
    }
}