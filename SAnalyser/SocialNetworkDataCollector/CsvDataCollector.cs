using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Domain.Interfaces;
using Domain.Models;

namespace SAnalyser.SocialNetworkDataCollector
{
    public class CsvDataCollector : IDataCollector
    {

        public async Task<DataContainer> CollectDataAsync(TaskInput taskInput)
        {
            IList<string> users = new List<string>();
            IList<string> texts = new List<string>();
            using (var reader = new StreamReader("tweets.csv"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<Tweet>();
                foreach (var record in records)
                {
                    users.Add(record.User);
                    texts.Add(record.Text);
                }
            }

            return new DataContainer
            {
                PostDataList = texts.Select(r => new PostData() {Text = r}).ToList(),
                UserDataList = users.Select(r => new UserData() {Name = r}).ToList()
            };
        }

        public class Tweet
        {
            public long Target { get; set; }
            public long Id { get; set; }
            public string Date { get; set; }
            public string Flag { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
        }
    }
}
