namespace Socneto.Domain.Models
{
    public class AnalysisValue
    {
        public AnalysisValueType ValueType { get; set; }
        
        public double Value { get; set; }
    }

    public enum AnalysisValueType
    {
        Number,
        String,
    }
}