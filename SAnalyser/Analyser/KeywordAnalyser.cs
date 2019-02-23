using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;

namespace SAnalyser.Analyser
{
    public class KeywordAnalyser : IAnalyser
    {
        public async Task<AnalysisResult> AnalyzeAsync(DataContainer dataContainer)
        {
            var texts = dataContainer.PostDataList.Select(r => r.Text);
            var keywords = getTopNKeywords(texts, 10);

            return new AnalysisResult()
            {
                AnalysedUserDataList = new List<AnalysedUserData>(),
                AnalysedPostDataList = new List<AnalysedPostData>(),
                Keywords = keywords

            };
        }


        private string[] getTopNKeywords(IEnumerable<string> texts, int n)
        {
            var keywords = new Dictionary<string, long>();

            foreach (var text in texts)
            {
                var alphanum = Regex.Replace(text, "[^a-zA-Z0-9]", "");
                var splitted = alphanum.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    foreach (var keyword in splitted)
                    {
                        var kwLower = keyword.ToLower();
                        long amount = 0;
                        if (keywords.TryGetValue(kwLower, out long retAmount))
                        {
                            keywords.Remove(kwLower);
                            amount = retAmount;
                        }

                        keywords.Add(kwLower, amount + 1);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

            return keywords.OrderByDescending(r => r.Value).Take(n).Select(r => r.Key).ToArray();
        }
    }
}
