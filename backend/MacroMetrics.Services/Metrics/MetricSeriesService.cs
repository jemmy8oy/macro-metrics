using Bogus;
using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Extensions;
using MacroMetrics.Abstractions.Services.Metrics;
using MacroMetrics.DomainModels.Models;

namespace MacroMetrics.Services.Metrics;

public class MetricSeriesService(
    IMetricCatalogueService    catalogueService,
    IDataNormalisationService  normalisationService) : IMetricSeriesService
{
    public IMetricSeries? GetSeries(string id, DateOnly? from = null, DateOnly? to = null)
    {
        var metadata = catalogueService.GetAll()
            .FirstOrDefault(m => m.Id.ToDisplayString() == id);

        if (metadata is null) return null;

        var rng    = new Randomizer(Math.Abs(id.GetHashCode()));
        var origin = metadata.EarliestDate.ToDateTime(TimeOnly.MinValue);
        var raw    = GenerateRawSeries(rng, origin, startValue: rng.Double(10, 100), volatility: 0.03);

        var normalised = normalisationService.NormaliseToMonthlyEndOfMonth(raw);

        var filtered = ApplyDateFilter(normalised, from, to);

        return new DomainMetricSeries
        {
            Id     = id,
            Label  = metadata.Label,
            Unit   = metadata.Unit.ToDisplayString(),
            Points = filtered
        };
    }

    private static IReadOnlyList<IMetricPoint> ApplyDateFilter(
        IReadOnlyList<IMetricPoint> points, DateOnly? from, DateOnly? to)
    {
        if (from is null && to is null)
            return points;

        return points
            .Where(p =>
            {
                var date = DateOnly.ParseExact(p.Date, "yyyy-MM-dd");
                if (from is not null && date < from.Value) return false;
                if (to   is not null && date > to.Value)   return false;
                return true;
            })
            .ToList();
    }

    private static List<DomainMetricPoint> GenerateRawSeries(
        Randomizer rng, DateTime origin, double startValue, double volatility)
    {
        var end    = DateTime.UtcNow;
        var months = ((end.Year - origin.Year) * 12) + end.Month - origin.Month;
        var points = new List<DomainMetricPoint>(months);
        var value  = startValue;

        for (int i = 0; i < months; i++)
        {
            value = Math.Max(0.01, value * (1 + rng.Double(-volatility, volatility)));
            points.Add(new DomainMetricPoint
            {
                Date  = origin.AddMonths(i).ToString("yyyy-MM-dd"),
                Value = Math.Round(value, 2)
            });
        }

        return points;
    }
}
