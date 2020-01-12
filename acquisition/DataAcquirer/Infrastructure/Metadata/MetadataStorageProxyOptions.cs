using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Infrastructure.Metadata
{
    public class MetadataStorageProxyOptions
    {
        [Required]
        public Uri BaseUri { get; set; }

        [Required]
        public string GetComponentJobMetadataRoute { get; set; }

        [Required]
        public string PostComponentMetadataRoute { get; set; }

    }
}
