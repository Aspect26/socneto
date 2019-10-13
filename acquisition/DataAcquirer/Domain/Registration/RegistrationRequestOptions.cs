using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Registration
{
    public class RegistrationRequestOptions
    {
        [Required]
        public string RegistrationChannelName { get; set; }

    }
}
