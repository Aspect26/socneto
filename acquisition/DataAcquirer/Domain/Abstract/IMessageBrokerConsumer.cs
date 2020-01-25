using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IMessageBrokerConsumer
    {
        Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken);
    }

    public interface ITranslationService
    {
        Task<string> TranslateToEnglishAsync(string fromLanguage, string text);
    }
}
