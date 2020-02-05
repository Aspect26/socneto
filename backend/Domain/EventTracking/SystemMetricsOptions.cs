using System.ComponentModel.DataAnnotations;

namespace Domain.EventTracking
{
    public class SystemMetricsOptions
    {
        [Required]
        public string SystemMetricsChannelName { get; set; }
    }
}
