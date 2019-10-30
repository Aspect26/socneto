using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ComponentIdentifiers
    {
        [Required]
        public string AnalyserComponentTypeName { get; set; }
        [Required]
        public string DataAcquirerComponentTypeName { get; set; }
    }
}