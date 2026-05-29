using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.Abstractions.Services.Metrics;

/// <summary>
/// Normalises raw metric data points to a consistent monthly end-of-month cadence.
/// </summary>
public interface IDataNormalisationService
{
    /// <summary>
    /// Takes an arbitrary sequence of data points (e.g. daily, weekly, or monthly)
    /// and returns exactly one point per calendar month, where the date is the last
    /// calendar day of that month and the value is the last available value within
    /// that month (i.e. the closing value).
    /// </summary>
    IReadOnlyList<IMetricPoint> NormaliseToMonthlyEndOfMonth(IEnumerable<IMetricPoint> points);
}
