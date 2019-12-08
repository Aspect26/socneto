using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Domain.Models
{
    public class ComponentModel
    {
        public string ComponentId { get; }
        public string UpdateChannelName { get; }
        public string ComponentType { get; }
        public string InputChannelName { get; }

        public JObject Attributes { get; }
        
        public ComponentModel(
            string componentId, 
            string requestComponentType,
            string inputChannelId,
            string updateChannelId, 
            JObject attributes)
        {
            ComponentId = componentId;
            UpdateChannelName = updateChannelId;
            InputChannelName = inputChannelId;
            ComponentType = requestComponentType;
            Attributes = attributes;
        }

    }
}