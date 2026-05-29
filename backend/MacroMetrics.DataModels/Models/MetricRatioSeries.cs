using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DataModels.Models;

public class MetricRatioSeries : IMetricRatioSeries
{
    public required string                      NumeratorId    { get; set; }
    public required string                      DenominatorId  { get; set; }
    public required IReadOnlyList<IMetricPoint> Points         { get; set; }
    public required double                      LongRunAverage { get; set; }
}
