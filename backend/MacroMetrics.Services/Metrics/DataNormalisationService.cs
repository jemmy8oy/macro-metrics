using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services.Metrics;
using MacroMetrics.DomainModels.Models;

namespace MacroMetrics.Services.Metrics;

/// <summary>
/// Normalises raw metric data points to a consistent monthly end-of-month cadence.
///
/// US-B5: Series data is normalised to monthly end-of-month cadence.
/// US-B10: Monthly end-of-month alignment for all source cadences.
/// </summary>
public class DataNormalisationService : IDataNormalisationService
{
    /// <inheritdoc />
    public IReadOnlyList<IMetricPoint> NormaliseToMonthlyEndOfMonth(IEnumerable<IMetricPoint> points)
    {
        return points
            .Where(p => DateOnly.TryParse(p.Date, out _))
            .GroupBy(p =>
            {
                var d = DateOnly.Parse(p.Date);
                return (d.Year, d.Month);
            })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                // Take the last available value within the month (closing price)
                var lastPoint = g.OrderBy(p => p.Date).Last();

                var (year, month) = g.Key;
                var lastDay       = DateTime.DaysInMonth(year, month);
                var eomDate       = new DateOnly(year, month, lastDay);

                return (IMetricPoint)new DomainMetricPoint
                {
                    Date  = eomDate.ToString("yyyy-MM-dd"),
                    Value = lastPoint.Value
                };
            })
            .ToList()
            .AsReadOnly();
    }
}
