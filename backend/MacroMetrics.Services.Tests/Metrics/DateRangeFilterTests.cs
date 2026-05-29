using MacroMetrics.Abstractions.Extensions;
using MacroMetrics.Services.Metrics;

namespace MacroMetrics.Services.Tests.Metrics;

/// <summary>
/// BDD-aligned tests derived from Issue #48 (US-B6).
///
/// Scenario: Series is filtered to the requested date range
///   Given a valid metric id "uk-house-prices" exists with data from 1968
///   When GET /api/metrics/uk-house-prices?from=2000-01-01&amp;to=2010-12-31 is called
///   Then the response status is 200
///   And all DataPoints have dates between 2000-01-31 and 2010-12-31 (inclusive, end-of-month aligned)
///   And no DataPoints outside this range are returned
///
/// Scenario: Series without date filter returns the full available history
///   Given a valid metric id "sp500" exists
///   When GET /api/metrics/sp500 is called with no from or to parameters
///   Then the response status is 200
///   And DataPoints start from the metric's earliestDate
/// </summary>
public class DateRangeFilterTests
{
    private readonly MetricSeriesService _sut =
        new(new MetricCatalogueService(), new DataNormalisationService());

    // ---------------------------------------------------------------
    // Scenario: Series is filtered to the requested date range
    // ---------------------------------------------------------------

    // And all DataPoints have dates between from and to (inclusive)

    [Fact]
    public void GetSeries_WithFromAndTo_ReturnsOnlyPointsWithinRange()
    {
        var from = new DateOnly(2000, 1, 1);
        var to   = new DateOnly(2010, 12, 31);

        var result = _sut.GetSeries("uk-house-prices", from, to);

        Assert.NotNull(result);
        Assert.All(result!.Points, p =>
        {
            var date = DateOnly.ParseExact(p.Date, "yyyy-MM-dd");
            Assert.True(date >= from, $"Date {p.Date} is before from={from}");
            Assert.True(date <= to,   $"Date {p.Date} is after to={to}");
        });
    }

    // And no DataPoints outside this range are returned

    [Fact]
    public void GetSeries_WithFromAndTo_ExcludesPointsBeforeFrom()
    {
        var from = new DateOnly(2000, 1, 1);
        var to   = new DateOnly(2010, 12, 31);

        var result   = _sut.GetSeries("uk-house-prices", from, to);
        var allDates = result!.Points.Select(p => DateOnly.ParseExact(p.Date, "yyyy-MM-dd")).ToList();

        Assert.DoesNotContain(allDates, d => d < from);
    }

    [Fact]
    public void GetSeries_WithFromAndTo_ExcludesPointsAfterTo()
    {
        var from = new DateOnly(2000, 1, 1);
        var to   = new DateOnly(2010, 12, 31);

        var result   = _sut.GetSeries("uk-house-prices", from, to);
        var allDates = result!.Points.Select(p => DateOnly.ParseExact(p.Date, "yyyy-MM-dd")).ToList();

        Assert.DoesNotContain(allDates, d => d > to);
    }

    // Filtered points should be a strict subset of the unfiltered series

    [Fact]
    public void GetSeries_WithDateRange_ReturnsFewerPointsThanFullSeries()
    {
        var full    = _sut.GetSeries("uk-house-prices");
        var filtered = _sut.GetSeries("uk-house-prices",
            from: new DateOnly(2000, 1, 1),
            to:   new DateOnly(2010, 12, 31));

        Assert.NotNull(full);
        Assert.NotNull(filtered);
        Assert.True(filtered!.Points.Count < full!.Points.Count,
            "Filtered series should have fewer points than the full series");
    }

    // ---------------------------------------------------------------
    // Scenario: Series without date filter returns the full available history
    // ---------------------------------------------------------------

    // And DataPoints start from the metric's earliestDate (EOM aligned)

    [Fact]
    public void GetSeries_NoFilter_FirstPointIsAtOrAfterEarliestDate()
    {
        var catalogue = new MetricCatalogueService();
        var sp500Meta = catalogue.GetAll()
            .First(m => m.Id.ToDisplayString() == "sp500");
        var result    = _sut.GetSeries("sp500");

        Assert.NotNull(result);
        var firstDate = DateOnly.ParseExact(result!.Points.First().Date, "yyyy-MM-dd");
        Assert.True(firstDate >= sp500Meta.EarliestDate,
            $"First point {firstDate} should be on or after earliestDate {sp500Meta.EarliestDate}");
    }

    [Fact]
    public void GetSeries_NoFilter_ReturnsNonEmptyPoints()
    {
        var result = _sut.GetSeries("sp500");

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Points);
    }

    // ---------------------------------------------------------------
    // Scenario: Only from specified — filter from date with no upper bound
    // ---------------------------------------------------------------

    [Fact]
    public void GetSeries_WithOnlyFrom_ExcludesPointsBeforeFrom()
    {
        var from   = new DateOnly(2015, 1, 1);
        var result = _sut.GetSeries("sp500", from: from);

        Assert.NotNull(result);
        Assert.All(result!.Points, p =>
        {
            var date = DateOnly.ParseExact(p.Date, "yyyy-MM-dd");
            Assert.True(date >= from, $"Date {p.Date} is before from={from}");
        });
    }

    // ---------------------------------------------------------------
    // Scenario: Only to specified — filter up to date with no lower bound
    // ---------------------------------------------------------------

    [Fact]
    public void GetSeries_WithOnlyTo_ExcludesPointsAfterTo()
    {
        var to     = new DateOnly(2005, 12, 31);
        var result = _sut.GetSeries("sp500", to: to);

        Assert.NotNull(result);
        Assert.All(result!.Points, p =>
        {
            var date = DateOnly.ParseExact(p.Date, "yyyy-MM-dd");
            Assert.True(date <= to, $"Date {p.Date} is after to={to}");
        });
    }

    // ---------------------------------------------------------------
    // Scenario: Unknown metric returns null regardless of filter
    // ---------------------------------------------------------------

    [Fact]
    public void GetSeries_UnknownId_WithDateRange_ReturnsNull()
    {
        var result = _sut.GetSeries("not-a-real-metric",
            from: new DateOnly(2000, 1, 1),
            to:   new DateOnly(2010, 12, 31));

        Assert.Null(result);
    }
}
