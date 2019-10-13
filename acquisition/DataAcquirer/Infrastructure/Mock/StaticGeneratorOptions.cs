using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DataGenerator
{
    public class StaticGeneratorOptions
    {
        [Required]
        public TimeSpan DownloadDelay { get; set; }
        [Required]
        public int Seed { get; set; }
        [Required]

        public string StaticDataPath { get; set; }
    }
}