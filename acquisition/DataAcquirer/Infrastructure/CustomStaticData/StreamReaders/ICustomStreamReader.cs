using System.IO;
using Domain.Model;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public interface ICustomStreamReader
    {
        void StartPopulating(Stream stream);
        bool ReadingEnded { get; }

        bool TryGetPost(out DataAcquirerPost post);
    }
}
