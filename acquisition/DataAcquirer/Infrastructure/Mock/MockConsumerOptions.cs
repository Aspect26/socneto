using System.Collections.Generic;

namespace Infrastructure.DataGenerator
{
    public class MockConsumerOptions
    {
        public string ConsumedTopic { get; set; }

        public string TopicQuery { get; set; }
        public Dictionary<string, string> CustomAttributes { get;set; }
    }
}
