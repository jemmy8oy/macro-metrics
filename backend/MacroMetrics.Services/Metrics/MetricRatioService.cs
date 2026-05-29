using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services.Metrics;
using MacroMetrics.DomainModels.Models;

namespace MacroMetrics.Services.Metrics;

public class MetricRatioService(IMetricSeriesService seriesService) : IMetricRatioService
{
    public IMetricRatioSeries? GetRatio(string numeratorId, string denominatorId)
    {
        var numerator   = seriesService.GetSeries(numeratorId);
        var denominator = seriesService.GetSeries(denominatorId);

        if (numerator is null || denominator is null) return null;

        // Build lookup of denominator points by date for O(1) access
        var denominatorByDate = denominator.Points
            .ToDictionary(p => p.Date, p => p.Value);

        // Compute ratio for dates present in both series (intersection)
        var ratioPoints = numerator.Points
            .Where(p => denominatorByDate.ContainsKey(p.Date) && denominatorByDate[p.Date] != 0)
            .Select(p => (IMetricPoint) new DomainMetricPoint
            {
                Date  = p.Date,
                Value = Math.Round(p.Value / denominatorByDate[p.Date], 4)
            })
            .OrderBy(p => p.Date)
            .ToList();

        var longRunAverage = ratioPoints.Count > 0
            ? Math.Round(ratioPoints.Average(p => p.Value), 4)
            : 0;

        return new DomainMetricRatioSeries
        {
            NumeratorId    = numeratorId,
            DenominatorId  = denominatorId,
            Points         = ratioPoints,
            LongRunAverage = longRunAverage
        };
    }
}
