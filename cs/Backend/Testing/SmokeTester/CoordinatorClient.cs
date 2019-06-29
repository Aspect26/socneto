using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SmokeTester
{
    public class CoordinatorClient
    {
        private readonly SmokeTesterOptions _smokeTesterOptions;
        public CoordinatorClient(IOptions<SmokeTesterOptions> smokeTesterOptionsObject)
        {
            if (string.IsNullOrEmpty(smokeTesterOptionsObject.Value.SubmitUri))
            {
                throw new ArgumentNullException(nameof(smokeTesterOptionsObject.Value.SubmitUri));
            }

            Console.WriteLine(smokeTesterOptionsObject.Value.SubmitUri);
            _smokeTesterOptions = smokeTesterOptionsObject.Value;
        }
        public async Task<string> Post(string json)
        {
            using (HttpClient client = new HttpClient())
            {

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_smokeTesterOptions.SubmitUri, stringContent);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}