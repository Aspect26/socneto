using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{
    public class MinioOptions
    {
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string AccessKey { get; set; }
        [Required]
        public string SecretKey { get; set; }
    }
}
