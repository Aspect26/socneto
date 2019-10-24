using System.ComponentModel.DataAnnotations;

namespace Domain.Registration
{
    public class RegistrationRequestValidationOptions
    {
        [Required]
        public string AnalyserOutputFormatElementName { get; set; }
    }
}