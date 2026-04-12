namespace MacroMetrics.DataModels.Models;

public record RatioResponse(
    string                   Numerator,
    string                   Denominator,
    double                   LongRunAverage,
    IReadOnlyList<DataPointDto> Series
);
