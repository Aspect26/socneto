using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ComponentOptions
    {
        [Required]
        public string ComponentId { get; set; }
    }
}