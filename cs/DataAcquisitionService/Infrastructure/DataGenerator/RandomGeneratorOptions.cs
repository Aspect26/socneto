using System;

namespace Infrastructure.DataGenerator
{
    public class RandomGeneratorOptions
    {
        public TimeSpan DownloadDelay { get; set; }
        public int Seed { get; set; }
    }

    public class StaticGeneratorOptions
    {
        public TimeSpan DownloadDelay { get; set; }
        public int Seed { get; set; }

        public string StaticDataPath { get; set; }
    }
}