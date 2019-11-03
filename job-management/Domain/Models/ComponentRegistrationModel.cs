using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Domain.Models
{
    public class ComponentRegistrationModel
    {
        public string ComponentId { get; }
        public string UpdateChannelId { get; }
        public string ComponentType { get; }
        public string InputChannelId { get; }

        public IReadOnlyDictionary<string,JObject> Attributes { get; }
        
        public ComponentRegistrationModel(
            string componentId, 
            string updateChannelId, 
            string inputChannelId,
            string requestComponentType,
            Dictionary<string,JObject> attributes)
        {
            ComponentId = componentId;
            UpdateChannelId = updateChannelId;
            InputChannelId = inputChannelId;
            ComponentType = requestComponentType;
            Attributes = attributes ?? new Dictionary<string, JObject>();
        }

    }
}