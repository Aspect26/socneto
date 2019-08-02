using System;
using System.Collections.Generic;

namespace Domain.Model
{
    public class Analysis
    {
        public Dictionary<String, Dictionary<String, AnalysisValue>> Data { get; set; }
    }

    public class AnalysisValue
    {
        // TODO: omg dynamic
        public dynamic Value { get; set; }
        
        public AnalysisValueType ValueType { get; set; }
        
    }

    public enum AnalysisValueType
    {
        Number, String, NumberList, StringList
    }
}