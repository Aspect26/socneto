namespace Socneto.Domain.Models
{
    public class ChartDefinition
    {
        public string DataJsonPath { get; set; }
        public ChartType ChartType { get; set; }
    }

    public enum ChartType
    {
        Line,
        Pie
    }
}