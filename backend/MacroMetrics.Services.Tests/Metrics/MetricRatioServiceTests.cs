using MacroMetrics.Services.Metrics;

namespace MacroMetrics.Services.Tests.Metrics;

/// <summary>
/// BDD-aligned tests derived from Issue #49 (US-B7).
///
/// Scenario: Ratio series computed correctly for two valid metrics
///   Given aligned monthly series exist for "us-house-prices" and "us-wages"
///   When GET /api/metrics/ratio?numerator=us-house-prices&amp;denominator=us-wages is called
///   Then the response status is 200
///   And the response body contains: numeratorId, denominatorId, points, and longRunAverage
///   And each DataPoint.value equals the numerator value divided by the denominator value for that date
///   And DataPoints only cover dates present in both series (the intersection of their date ranges)
///   And dates are in chronological ascending order
/// </summary>
public class MetricRatioServiceTests
{
    private readonly MetricSeriesService _seriesService = new(new MetricCatalogueService());

    private MetricRatioService CreateSut() => new(_seriesService);

    // --- Happy path: two valid metrics ---

    [Fact]
    public void GetRatio_TwoValidMetrics_ReturnsNonNull()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages");

        Assert.NotNull(result);
    }

    [Fact]
    public void GetRatio_TwoValidMetrics_NumeratorIdMatchesRequest()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages");

        Assert.Equal("us-house-prices", result!.NumeratorId);
    }

    [Fact]
    public void GetRatio_TwoValidMetrics_DenominatorIdMatchesRequest()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages");

        Assert.Equal("us-wages", result!.DenominatorId);
    }

    [Fact]
    public void GetRatio_TwoValidMetrics_PointsArrayIsNotEmpty()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages");

        Assert.NotEmpty(result!.Points);
    }

    // And the response body contains: longRunAverage

    [Fact]
    public void GetRatio_TwoValidMetrics_LongRunAverageIsPositive()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages");

        Assert.True(result!.LongRunAverage > 0,
            $"Expected positive longRunAverage but got {result.LongRunAverage}");
    }

    // And each DataPoint.value equals numerator / denominator for that date

    [Fact]
    public void GetRatio_TwoValidMetrics_EachPointValueEqualsNumeratorDividedByDenominator()
    {
        var sut         = CreateSut();
        var numerator   = _seriesService.GetSeries("us-house-prices")!;
        var denominator = _seriesService.GetSeries("us-wages")!;
        var result      = sut.GetRatio("us-house-prices", "us-wages")!;

        var numByDate   = numerator.Points.ToDictionary(p => p.Date, p => p.Value);
        var denByDate   = denominator.Points.ToDictionary(p => p.Date, p => p.Value);

        Assert.All(result.Points, p =>
        {
            Assert.True(numByDate.ContainsKey(p.Date),  $"Date {p.Date} not found in numerator");
            Assert.True(denByDate.ContainsKey(p.Date),  $"Date {p.Date} not found in denominator");
            var expected = Math.Round(numByDate[p.Date] / denByDate[p.Date], 4);
            Assert.Equal(expected, p.Value);
        });
    }

    // And DataPoints only cover dates present in both series (intersection)

    [Fact]
    public void GetRatio_TwoValidMetrics_PointDatesAreIntersectionOfBothSeries()
    {
        var sut         = CreateSut();
        var numerator   = _seriesService.GetSeries("us-house-prices")!;
        var denominator = _seriesService.GetSeries("us-wages")!;
        var result      = sut.GetRatio("us-house-prices", "us-wages")!;

        var numDates    = numerator.Points.Select(p => p.Date).ToHashSet();
        var denDates    = denominator.Points.Select(p => p.Date).ToHashSet();
        var intersection = numDates.Intersect(denDates).ToHashSet();

        Assert.All(result.Points, p => Assert.Contains(p.Date, intersection));
    }

    // And dates are in chronological ascending order

    [Fact]
    public void GetRatio_TwoValidMetrics_DatesAreInAscendingOrder()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages")!;
        var dates  = result.Points.Select(p => p.Date).ToList();
        var sorted = dates.OrderBy(d => d).ToList();

        Assert.Equal(sorted, dates);
    }

    // And all point dates are valid ISO format

    [Fact]
    public void GetRatio_TwoValidMetrics_AllPointDatesAreIsoFormat()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages")!;

        Assert.All(result.Points, p =>
            Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", p.Date));
    }

    // And all point values are positive

    [Fact]
    public void GetRatio_TwoValidMetrics_AllPointValuesArePositive()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "us-wages")!;

        Assert.All(result.Points, p =>
            Assert.True(p.Value > 0, $"Expected positive value but got {p.Value}"));
    }

    // And longRunAverage equals the mean of all ratio point values

    [Fact]
    public void GetRatio_TwoValidMetrics_LongRunAverageEqualsMeanOfPoints()
    {
        var sut      = CreateSut();
        var result   = sut.GetRatio("us-house-prices", "us-wages")!;
        var expected = Math.Round(result.Points.Average(p => p.Value), 4);

        Assert.Equal(expected, result.LongRunAverage);
    }

    // --- Unknown numerator returns null (maps to 404) ---

    [Fact]
    public void GetRatio_UnknownNumerator_ReturnsNull()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("not-a-real-metric", "us-wages");

        Assert.Null(result);
    }

    // --- Unknown denominator returns null (maps to 404) ---

    [Fact]
    public void GetRatio_UnknownDenominator_ReturnsNull()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("us-house-prices", "not-a-real-metric");

        Assert.Null(result);
    }

    // --- Both unknown returns null ---

    [Fact]
    public void GetRatio_BothUnknown_ReturnsNull()
    {
        var sut    = CreateSut();
        var result = sut.GetRatio("foo", "bar");

        Assert.Null(result);
    }

    // --- Determinism: same pair always produces same ratio series ---

    [Fact]
    public void GetRatio_SamePair_IsDeterministic()
    {
        var sut    = CreateSut();
        var first  = sut.GetRatio("us-house-prices", "us-wages")!.Points.ToList();
        var second = sut.GetRatio("us-house-prices", "us-wages")!.Points.ToList();

        Assert.Equal(first.Count, second.Count);
        for (int i = 0; i < first.Count; i++)
        {
            Assert.Equal(first[i].Date,  second[i].Date);
            Assert.Equal(first[i].Value, second[i].Value);
        }
    }

    // --- Various valid metric pairs all return non-null ---

    [Theory]
    [InlineData("us-house-prices", "us-wages")]
    [InlineData("gold", "oil")]
    [InlineData("sp500", "us-cpi")]
    [InlineData("uk-house-prices", "uk-wages")]
    public void GetRatio_ValidMetricPairs_ReturnNonNull(string numeratorId, string denominatorId)
    {
        var sut    = CreateSut();
        var result = sut.GetRatio(numeratorId, denominatorId);

        Assert.NotNull(result);
    }
}
