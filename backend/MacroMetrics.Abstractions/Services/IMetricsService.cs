namespace MacroMetrics.Abstractions.Services;

public record DataPoint(string Date, double Value);
public record RatioResponse(string Numerator, string Denominator, double LongRunAverage, IReadOnlyList<DataPoint> Series);
public record IndicatorResponse(string Id, string Label, string Unit, double LongRunAverage, IReadOnlyList<DataPoint> Series);

public interface IMetricsService
{
    RatioResponse GetRatio(string numerator, string denominator);
    IndicatorResponse? GetIndicator(string id);
}
