using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Infrastructure.StaticData;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataGenerator
{
    public sealed class MovieDataProvider : IStaticDataProvider, IDisposable
    {
        private readonly string _staticDataPath;
        private StreamReader _streamReader;
        private CsvReader _csvReader;

        public MovieDataProvider(
            IOptions<StaticDataOptions> staticGeneratorOptionsAccessor)
        {
            _staticDataPath = staticGeneratorOptionsAccessor.Value.StaticDataPath;
        }

        public IEnumerable<UniPostStaticData> GetEnumerable()
        {
            _streamReader = new StreamReader(_staticDataPath);

            _csvReader = new CsvReader(_streamReader);

            var enumerable = _csvReader.GetRecords<MovieSetEntity>()
                                        .Select(r =>
                    {
                        var id = Guid.NewGuid();
                        return new UniPostStaticData(
                            id,
                            $"movie_{id}",
                            r.Text,
                            "MOVIE_DATASET",
                            r.User,
                            DateTime.Now.ToString("s"),
                            "q");
                    });

            return enumerable;
        }

        public void Dispose()
        {
            _csvReader?.Dispose();
            _streamReader?.Dispose();
        }
    }

}
