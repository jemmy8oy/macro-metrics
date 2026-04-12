namespace MacroMetrics.DataModels.Models;

public record IndicatorResponse(
    string                      Id,
    string                      Label,
    string                      Unit,
    double                      LongRunAverage,
    IReadOnlyList<DataPointDto> Series
);
