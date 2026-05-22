using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Enums;

namespace MacroMetrics.DataModels.Models;

public class MetricMetadata : IMetricMetadata
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public MetricSource Source { get; set; }
    public bool IsIndicatorOnly { get; set; }
    public DateOnly EarliestDate { get; set; }
}
