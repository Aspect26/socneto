using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ComponentOptions
    {
        [Required]
        public string ComponentType { get; set; }
        [Required]
        public string ComponentId { get; set; }

        [Required]
        public string InputChannelName { get; set; }

        [Required]
        public string UpdateChannelName { get; set; }
    }
}