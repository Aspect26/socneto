using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.Extensions.Options;

namespace Domain.Analyser
{

    public interface IAnalyser
    {
        Task<Analysis> AnalyzePost(UniPost post);
    }
    
    public class MockAnalyser : IAnalyser
    {
        private Dictionary<string, double> _emoticonsPolarity = new Dictionary<string, double>
        {
            { ":D", 1.0f },
            { ":)", 1.0f },
            { ":|", 0.0f },
            { ":(", -1.0f },
            { ":'", -1.0f },
        };
        
        private Dictionary<string, double> _emoticonsAccuracy = new Dictionary<string, double>
        {
            { ":D", 1.0f },
            { ":)", 0.8f },
            { ":|", 1.0f },
            { ":(", 0.8f },
            { ":'", 1.0f },
        };
        
        public Task<Analysis> AnalyzePost(UniPost post)
        {
            var random = new Random(post.Text.GetHashCode());
            double polarity;
            double accuracy;

            var knownEmoticonInText = GetKnownEmoticon(post.Text);
            
            if (knownEmoticonInText != null)
            {
                (polarity, accuracy) = AnalyzeEmoticon(knownEmoticonInText);
            }
            else
            {
                (polarity, accuracy) = AnalyzeRandom(random);
            }

            var analysis = new Analysis
            {
                Data = new Dictionary<string, AnalysisResult>
                {
                    ["polarity"] = new AnalysisResult
                    {
                        NumberValue = polarity
                    },
                    ["accuracy"] = new AnalysisResult
                    {
                        NumberValue = accuracy
                    }
                }
            };

            return Task.FromResult(analysis);
        }

        private string GetKnownEmoticon(string text)
        {
            var knownEmoticons = _emoticonsPolarity.Keys;
            foreach (var knownEmoticon in knownEmoticons)
            {
                if (text.Contains(knownEmoticon))
                {
                    return knownEmoticon;
                }
            }

            return null;
        }

        private (double polarity, double accuracy) AnalyzeEmoticon(string emoticon)
        {
            _emoticonsPolarity.TryGetValue(emoticon, out var polarity);
            _emoticonsAccuracy.TryGetValue(emoticon, out var accuracy);
            
            return (polarity, accuracy);
        }
        
        private (double polarity, double accuracy) AnalyzeRandom(Random random)
        {
            return ((random.NextDouble() - 0.5) * 2, random.NextDouble());
        }
    }
}