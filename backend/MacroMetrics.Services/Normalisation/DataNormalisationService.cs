using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services.Normalisation;
using MacroMetrics.DomainModels.Models;

namespace MacroMetrics.Services.Normalisation;

/// <summary>
/// Normalises raw metric data points — regardless of their original cadence
/// (daily, monthly mid-month, etc.) — to a single end-of-month point per
/// calendar month, suitable for aligned ratio computation.
///
/// After alignment, any calendar months with no source data are forward-filled
/// using the preceding month's value (carry-forward). Leading gaps (months
/// before the first available data point) are dropped, not filled.
/// </summary>
public class DataNormalisationService : IDataNormalisationService
{
    /// <inheritdoc />
    public IReadOnlyList<IMetricPoint> Normalise(IEnumerable<IMetricPoint> rawPoints)
    {
        // Step 1: Group by (year, month), take the last value within each group,
        // and output a single point dated to the last calendar day of that month.
        var aligned = rawPoints
            .Where(p => DateOnly.TryParseExact(p.Date, "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out _))
            .Select(p => (
                Point: p,
                Parsed: DateOnly.ParseExact(p.Date, "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture)))
            .OrderBy(x => x.Parsed)
            .GroupBy(x => new { x.Parsed.Year, x.Parsed.Month })
            .Select(g =>
            {
                // Last value within the month (closing / latest price)
                var last       = g.Last();
                var endOfMonth = new DateOnly(g.Key.Year, g.Key.Month,
                                     DateTime.DaysInMonth(g.Key.Year, g.Key.Month));

                return (IMetricPoint)new DomainMetricPoint
                {
                    Date  = endOfMonth.ToString("yyyy-MM-dd"),
                    Value = last.Point.Value
                };
            })
            .ToList();

        // Step 2: Forward-fill any gaps between consecutive months.
        // Leading gaps (before the first data point) are not filled.
        return ForwardFillGaps(aligned).AsReadOnly();
    }

    /// <summary>
    /// Iterates through a chronologically sorted list of end-of-month points and
    /// inserts a carry-forward point for every calendar month that has no entry
    /// between two consecutive existing points.
    /// </summary>
    private static List<IMetricPoint> ForwardFillGaps(List<IMetricPoint> aligned)
    {
        if (aligned.Count == 0)
            return aligned;

        var result = new List<IMetricPoint>(aligned.Count);

        foreach (var current in aligned)
        {
            if (result.Count > 0)
            {
                // Walk forward one month at a time from the last emitted point
                // until we reach the month of 'current', filling any gaps.
                var lastDate    = DateOnly.ParseExact(result[^1].Date, "yyyy-MM-dd",
                                      System.Globalization.CultureInfo.InvariantCulture);
                var currentDate = DateOnly.ParseExact(current.Date, "yyyy-MM-dd",
                                      System.Globalization.CultureInfo.InvariantCulture);

                var fillMonth = new DateOnly(lastDate.Year, lastDate.Month, 1).AddMonths(1);
                var stopMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);

                while (fillMonth < stopMonth)
                {
                    var endOfFillMonth = new DateOnly(fillMonth.Year, fillMonth.Month,
                                             DateTime.DaysInMonth(fillMonth.Year, fillMonth.Month));

                    result.Add(new DomainMetricPoint
                    {
                        Date  = endOfFillMonth.ToString("yyyy-MM-dd"),
                        Value = result[^1].Value   // carry the last known value forward
                    });

                    fillMonth = fillMonth.AddMonths(1);
                }
            }

            result.Add(current);
        }

        return result;
    }
}
