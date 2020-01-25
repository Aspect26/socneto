using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Infrastructure.Translation
{
    public class TranslationServiceOptions
    {
        [Required]
        public string SubscriptionKey { get; set; }

        [Required]
        public string Endpoint { get; set; }

    }
}

