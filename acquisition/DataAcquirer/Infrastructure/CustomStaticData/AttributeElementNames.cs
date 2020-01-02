using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{
    public class AttributeElementNames
    {
        [Required]
        public string BucketElementName { get; set; }
        [Required]
        public string ObjectElementName { get; set; }
        [Required]
        public string MappingElementName { get; set; }
    }
}
