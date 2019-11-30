using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Domain
{
    public class LogLevelOptions
    {
        public LogLevel? Default { get; set; }
    }

    public class SystemMetricsOptions
    {
        [Required]
        public string SystemMetricsChannelName { get; set; }
    }
}
