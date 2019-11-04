using System.Collections.Generic;

namespace Domain
{
    public class ComponentOptions
    {
        public string ComponentType { get; set; }
        public string ComponentId { get; set; }
        public string InputChannelName { get; set; }
        public string UpdateChannelName { get; set; }
        public Dictionary<string, Dictionary<string, string>> Attributes { get; set; }
    }
}