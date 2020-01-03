using System;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Domain;
using Domain.Model;
using Infrastructure.CustomStaticData.MappingAttributes;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class CsvStreamReader : BaseStreamReader, ICustomStreamReader
    {
        private readonly CsvMappingAttributes _attributes;

        public CsvStreamReader(CsvMappingAttributes attributes)
        {
            _attributes = attributes;
        }

        public void StartPopulating(Stream stream)
        {
            try
            {
                using var se = new StreamReader(stream);
                using var csvreader = new CsvReader(se);

                if (_attributes.HasHeaders)
                {
                    csvreader.Read();
                }

                while (csvreader.Read())
                {
                    var dataAcquirerPost = ParsePostFromReader(csvreader);
                    _posts.Add(dataAcquirerPost);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _posts.CompleteAdding();
            }
        }

        private DataAcquirerPost ParsePostFromReader(CsvReader csvreader)
        {
            var builder = new PostBuilder(_attributes.DateTimeFormatString)
                .AddSource()
                .PopulateFixed(_attributes.FixedValues);

            foreach (var (k, index) in _attributes.Indices)
            {
                if (csvreader.TryGetField<string>(index, out var value))
                {
                    builder.PopulateField(k, value);
                }
            }

            return builder.Build();
        }
    }
}
