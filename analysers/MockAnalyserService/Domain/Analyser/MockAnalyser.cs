using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Analyser
{

    public interface IAnalyser
    {
        Task<Analysis> AnalyzePost(UniPost post);
    }
    
    public class MockAnalyser : IAnalyser
    {
        public Task<Analysis> AnalyzePost(UniPost post)
        {
            var random = new Random(post.Text.GetHashCode());

            var analysis = new Analysis
            {
                Data = new Dictionary<string, AnalysisValue>
                {
                    ["polarity"] = new AnalysisValue
                    {
                        Value = (random.NextDouble() - 0.5) * 2, ValueType = AnalysisValueType.Number
                    },
                    ["accuracy"] = new AnalysisValue
                    {
                        Value = random.NextDouble()
                    }
                }
            };

            return Task.FromResult(analysis);
        }
    }
}