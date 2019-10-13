using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DataGenerator
{
    public class RandomGeneratorOptions
    {

        [Required]
        public TimeSpan DownloadDelay { get; set; }

        [Required]
        public int Seed { get; set; }
    }
}