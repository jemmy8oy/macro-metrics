using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DataModels.Models;

public class MetricMetadata : IMetricMetadata
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsIndicatorOnly { get; set; }
    public DateOnly EarliestDate { get; set; }
}
