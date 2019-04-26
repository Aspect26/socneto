using System;
using System.Threading.Tasks;

namespace TestCases
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            await new ProducerTestCase().PerformTestCase();
        }

    }
}
