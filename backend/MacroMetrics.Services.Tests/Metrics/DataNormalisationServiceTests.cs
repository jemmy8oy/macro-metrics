using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.DataModels.Models;
using MacroMetrics.Services.Metrics;

namespace MacroMetrics.Services.Tests.Metrics;

/// <summary>
/// BDD-aligned tests derived from Issue #47 (US-B5).
///
/// Scenario: Daily source data is normalised to monthly cadence
///   Given the yfinance sidecar returns raw daily closing data for "gold"
///   When GET /api/metrics/gold is called
///   Then the response DataPoints are at monthly cadence
///   And no two DataPoints share the same calendar month
///   And each DataPoint date is the last day of its calendar month (e.g. 2024-01-31, 2024-02-29)
/// </summary>
public class DataNormalisationServiceTests
{
    private readonly DataNormalisationService _sut = new();

    // --- Helper: build a MetricPoint ---
    private static IMetricPoint Point(string date, double value) =>
        new MetricPoint { Date = date, Value = value };

    // ---------------------------------------------------------------
    // Then the response DataPoints are at monthly cadence
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_DailyInput_ProducesOnePointPerCalendarMonth()
    {
        // Given: 31 daily closing prices for January 2024
        var daily = Enumerable.Range(1, 31)
            .Select(d => Point($"2024-01-{d:D2}", d * 10.0))
            .ToList();

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Single(result);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_MultiMonthDailyInput_CountEqualsMonthSpan()
    {
        // Given: daily prices across Jan and Feb 2024 (31 + 29 = 60 days)
        var jan = Enumerable.Range(1, 31).Select(d => Point($"2024-01-{d:D2}", d));
        var feb = Enumerable.Range(1, 29).Select(d => Point($"2024-02-{d:D2}", d + 100));
        var daily = jan.Concat(feb).ToList();

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Equal(2, result.Count);
    }

    // ---------------------------------------------------------------
    // And each DataPoint date is the last day of its calendar month
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_DailyInput_EachDateIsLastDayOfMonth()
    {
        var daily = Enumerable.Range(1, 31).Select(d => Point($"2024-01-{d:D2}", d));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Equal("2024-01-31", result[0].Date);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_LeapYearFebruary_DateIs29th()
    {
        // 2024 is a leap year — last day of Feb is the 29th
        var daily = Enumerable.Range(1, 29).Select(d => Point($"2024-02-{d:D2}", d));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Equal("2024-02-29", result[0].Date);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_NonLeapYearFebruary_DateIs28th()
    {
        // 2023 is not a leap year — last day of Feb is the 28th
        var daily = Enumerable.Range(1, 28).Select(d => Point($"2023-02-{d:D2}", d));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Equal("2023-02-28", result[0].Date);
    }

    // ---------------------------------------------------------------
    // And no two DataPoints share the same calendar month
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_DailyInput_NoTwoPointsShareSameCalendarMonth()
    {
        var daily = Enumerable.Range(1, 31).Select(d => Point($"2024-03-{d:D2}", d))
            .Concat(Enumerable.Range(1, 30).Select(d => Point($"2024-04-{d:D2}", d + 50)));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        var months = result.Select(p => p.Date[..7]).ToList();  // "YYYY-MM"
        Assert.Equal(months.Distinct().Count(), months.Count);
    }

    // ---------------------------------------------------------------
    // Value selection: last closing value within the month
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_DailyInput_ValueIsLastDayClosingPrice()
    {
        // Given: Jan 2024 daily data — last day (31st) has value 999.99
        var daily = Enumerable.Range(1, 30).Select(d => Point($"2024-01-{d:D2}", d * 1.0))
            .Append(Point("2024-01-31", 999.99));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.Equal(999.99, result[0].Value, precision: 2);
    }

    // ---------------------------------------------------------------
    // Already-monthly data: align mid-month dates to end-of-month
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_MidMonthDates_AreReplacedWithEndOfMonth()
    {
        // Given: monthly data on the 1st of each month
        var monthly = new[]
        {
            Point("2024-01-01", 100.0),
            Point("2024-02-01", 200.0),
            Point("2024-03-01", 300.0)
        };

        var result = _sut.NormaliseToMonthlyEndOfMonth(monthly);

        Assert.Equal("2024-01-31", result[0].Date);
        Assert.Equal("2024-02-29", result[1].Date);
        Assert.Equal("2024-03-31", result[2].Date);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_MidMonthDates_ValuesArePreserved()
    {
        var monthly = new[]
        {
            Point("2024-01-15", 150.0),
            Point("2024-02-15", 250.0)
        };

        var result = _sut.NormaliseToMonthlyEndOfMonth(monthly);

        Assert.Equal(150.0, result[0].Value);
        Assert.Equal(250.0, result[1].Value);
    }

    // ---------------------------------------------------------------
    // Edge cases
    // ---------------------------------------------------------------

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_EmptyInput_ReturnsEmptyList()
    {
        var result = _sut.NormaliseToMonthlyEndOfMonth(Array.Empty<IMetricPoint>());

        Assert.Empty(result);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_OutputIsInChronologicalAscendingOrder()
    {
        // Input deliberately out of order
        var points = new[]
        {
            Point("2024-03-15", 300.0),
            Point("2024-01-15", 100.0),
            Point("2024-02-15", 200.0)
        };

        var result = _sut.NormaliseToMonthlyEndOfMonth(points);

        var dates  = result.Select(p => p.Date).ToList();
        var sorted = dates.OrderBy(d => d).ToList();
        Assert.Equal(sorted, dates);
    }

    [Fact]
    public void NormaliseToMonthlyEndOfMonth_AllOutputDatesMatchIsoFormat()
    {
        var daily = Enumerable.Range(1, 31).Select(d => Point($"2024-05-{d:D2}", d));

        var result = _sut.NormaliseToMonthlyEndOfMonth(daily);

        Assert.All(result, p => Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", p.Date));
    }
}
