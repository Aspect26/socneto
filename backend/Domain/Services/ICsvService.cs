using System.Collections.Generic;

namespace Socneto.Domain.Services
{
    public interface ICsvService
    {
        string GetCsv<T>(IEnumerable<T> data, bool withHeaders);
    }
}