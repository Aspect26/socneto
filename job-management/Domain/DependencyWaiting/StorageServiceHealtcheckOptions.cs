using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DependencyWaiting
{
    public class StorageServiceHealtcheckOptions
    {
        [Required]
        public Uri BaseUri { get; set; }

        [Required]
        public string HealthCheckEndpoint { get; set; }
    }


}
