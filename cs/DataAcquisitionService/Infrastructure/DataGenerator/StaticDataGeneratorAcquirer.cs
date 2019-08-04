using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Domain.Acquisition;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataGenerator
{ 
    public class StaticDataGeneratorAcquirer : IDataAcquirer
    {
        private readonly ILogger<StaticDataGeneratorAcquirer> _logger;
        private readonly Random _random;
        private readonly TimeSpan _downloadDelay;
        private readonly string _staticDataPath;

        private IList<UniPost> _records = null;
        private class MovieSetEntity
        {
            public string Target { get; set; }

            public string Id { get; set; }

            public string Date { get; set; }

            public string Flag { get; set; }

            public string User { get; set; }

            public string Text { get; set; }

        }

        public StaticDataGeneratorAcquirer(
            ILogger<StaticDataGeneratorAcquirer> logger,
            IOptions<StaticGeneratorOptions> randomGenratorOptionsAccessor
            )
        {
            _logger = logger;
            _downloadDelay = randomGenratorOptionsAccessor.Value.DownloadDelay;
            _random = new Random(randomGenratorOptionsAccessor.Value.Seed);
            _staticDataPath = randomGenratorOptionsAccessor.Value.StaticDataPath;
        }

        public async Task<DataAcquirerOutputModel> AcquireBatchAsync(
            DataAcquirerInputModel acquirerInputModel,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Collecting stuff");
            var waitTask = Task.Delay(_downloadDelay);

            if (_records == null)
            {
                _logger.LogInformation("Parsing results");

                using (var reader = new StreamReader(_staticDataPath))
                using (var csv = new CsvReader(reader))
                {
                    _records = csv.GetRecords<MovieSetEntity>()
                        .Select(r => UniPost.FromValues(
                            Guid.NewGuid().ToString(),
                                r.Text,
                                "STATIC_TEST",
                                r.User,
                                DateTime.Now.ToString("s"),
                        acquirerInputModel.JobId))
                        .ToList();
                }

            _logger.LogInformation("Parsing done");
            }

            await waitTask;


            var random100 = Enumerable
                .Range(0, 100)
                .Select(r => _random.Next(0, _records.Count))
                .Select(r => _records[r])
                .ToList();

            return new DataAcquirerOutputModel
            {
                Posts = random100
            };
        }
    }
}
