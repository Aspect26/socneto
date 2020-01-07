using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SystemMetricsOptions
    {
        [Required]
        public string SystemMetricsChannelName { get; set; }
    }
}
