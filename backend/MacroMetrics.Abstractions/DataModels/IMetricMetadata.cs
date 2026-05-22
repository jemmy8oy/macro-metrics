namespace MacroMetrics.Abstractions.DataModels;

public interface IMetricMetadata
{
    string Id { get; set; }
    string Label { get; set; }
    string Unit { get; set; }
    string Source { get; set; }
    bool IsIndicatorOnly { get; set; }
    DateOnly EarliestDate { get; set; }
}
