using System.ComponentModel.DataAnnotations;

namespace Domain.Registration
{
    public class RegistrationRequestOptions
    {
        [Required]
        public string RegistrationChannelName { get; set; }
    }
}