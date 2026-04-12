using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DomainModels.Models;

public record Ratio(
    string                    Numerator,
    string                    Denominator,
    double                    LongRunAverage,
    IReadOnlyList<IDataPoint> Series
) : IRatio;
