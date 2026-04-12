using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DomainModels.Models;

public record Indicator(
    string                    Id,
    string                    Label,
    string                    Unit,
    double                    LongRunAverage,
    IReadOnlyList<IDataPoint> Series
) : IIndicator;
