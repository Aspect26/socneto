using System.Collections.Generic;

namespace Domain.Models
{
    public class ComponentRegistrationModel
    {
        public string ComponentId { get; }
        public string UpdateChannelId { get; }
        public string ComponentType { get; }
        public string InputChannelId { get; }

        public IReadOnlyDictionary<string,string> Attributes { get; }
        
        public ComponentRegistrationModel(
            string componentId, 
            string updateChannelId, 
            string inputChannelId,
            string requestComponentType,
            Dictionary<string,string> attributes)
        {
            ComponentId = componentId;
            UpdateChannelId = updateChannelId;
            InputChannelId = inputChannelId;
            ComponentType = requestComponentType;
            Attributes = attributes;
        }

    }
}