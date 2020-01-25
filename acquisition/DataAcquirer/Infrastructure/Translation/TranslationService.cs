using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Translation
{
    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TranslationService> _logger;
        private readonly string _endpoint;
        private readonly string _subscriptionKey;

        public TranslationService(
            HttpClient httpClient,
            IOptions<TranslationServiceOptions> translationServiceOptionsAccessor,
            ILogger<TranslationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _endpoint = translationServiceOptionsAccessor.Value.Endpoint;
            _subscriptionKey = translationServiceOptionsAccessor.Value.SubscriptionKey;
        }

        public async Task<string> TranslateToEnglishAsync(string fromLanguage, string text)
        {
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                var formatted = string.Format(_endpoint, fromLanguage);
                request.RequestUri = new Uri(formatted);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                var response = await _httpClient.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();

                try
                {
                    var deserializedOutput = JsonConvert
                        .DeserializeObject<TranslationResult[]>(result);
                    if (deserializedOutput.Length == 0)
                    {
                        throw new DataAcquirerException("Invalid translator response");
                    }
                    var o = deserializedOutput[0];
                    if (o.Translations.Length == 0)
                    {
                        throw new DataAcquirerException("Invalid translator response");
                    }
                    return o.Translations[0].Text;

                }
                catch (JsonException je)
                {
                    throw new DataAcquirerException("Could not deserialize translation.", je);
                }
            }
        }



    }
}

