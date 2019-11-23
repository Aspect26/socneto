using System.Collections.Generic;

namespace Infrastructure.DataGenerator
{
    public class MockConsumerOptions
    {
        public string ConsumedTopic { get; set; }

        public List<string> Topics { get; set; }
        public Dictionary<string, string> CustomAttributes { get;set; }
    }
}
