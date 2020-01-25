using System.Net.Http;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Exceptions;

namespace Infrastructure.Translation
{
    public class NullTranslationService : ITranslationService
    {
        public NullTranslationService(HttpClient httpClient)
        {
            // ignored
        }
        public Task<string> TranslateToEnglishAsync(string fromLanguage, string text)
        {
            throw new DataAcquirerException("This service does not translate");
        }
    }
}

