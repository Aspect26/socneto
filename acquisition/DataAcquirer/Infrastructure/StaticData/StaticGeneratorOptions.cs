using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.StaticData
{
    public class StaticGeneratorOptions
    {
        [Required]
        public TimeSpan DownloadDelay { get; set; }
        
        [Required]

        public string StaticDataPath { get; set; }
    }
}