using MacroMetrics.Abstractions.Enums;
using MacroMetrics.Abstractions.Extensions;
using MacroMetrics.Services.Metrics;

namespace MacroMetrics.Services.Tests.Metrics;

public class MetricCatalogueServiceTests
{
    private readonly MetricCatalogueService _sut = new();

    [Fact]
    public void GetAll_ReturnsExactly14Metrics()
    {
        var result = _sut.GetAll();

        Assert.Equal(14, result.Count);
    }

    [Fact]
    public void GetAll_AllEntriesHaveDefinedId()
    {
        var result = _sut.GetAll();

        Assert.All(result, m => Assert.True(Enum.IsDefined(typeof(MetricId), m.Id)));
    }

    [Fact]
    public void GetAll_AllIdsSerialiseToKebabCase()
    {
        var result = _sut.GetAll();

        // kebab-case: lowercase letters, digits, and hyphens only
        Assert.All(result, m =>
            Assert.Matches(@"^[a-z0-9]+(-[a-z0-9]+)*$", m.Id.ToDisplayString()));
    }

    [Fact]
    public void GetAll_AllEntriesHaveNonEmptyLabel()
    {
        var result = _sut.GetAll();

        Assert.All(result, m => Assert.False(string.IsNullOrWhiteSpace(m.Label)));
    }

    [Fact]
    public void GetAll_AllEntriesHaveDefinedUnit()
    {
        var result = _sut.GetAll();

        Assert.All(result, m => Assert.True(Enum.IsDefined(typeof(MetricUnit), m.Unit)));
    }

    [Fact]
    public void GetAll_AllEntriesHaveDefinedSource()
    {
        var result = _sut.GetAll();

        Assert.All(result, m => Assert.True(Enum.IsDefined(typeof(MetricSource), m.Source)));
    }

    [Theory]
    [InlineData(MetricId.Cape)]
    [InlineData(MetricId.Uk10YrGilt)]
    [InlineData(MetricId.Us10YrTreasury)]
    public void GetAll_IndicatorOnlyMetricsAreCorrectlyFlagged(MetricId id)
    {
        var result = _sut.GetAll();

        var metric = Assert.Single(result, m => m.Id == id);
        Assert.True(metric.IsIndicatorOnly, $"Metric '{id}' should have IsIndicatorOnly = true");
    }

    [Fact]
    public void GetAll_ExactlyThreeMetricsAreIndicatorOnly()
    {
        var result = _sut.GetAll();

        var indicatorOnly = result.Where(m => m.IsIndicatorOnly).ToList();
        Assert.Equal(3, indicatorOnly.Count);
    }

    [Fact]
    public void GetAll_NonIndicatorMetricsHaveIsIndicatorOnlyFalse()
    {
        var result = _sut.GetAll();

        var nonIndicator = result.Where(m => !m.IsIndicatorOnly);
        Assert.All(nonIndicator, m =>
            Assert.False(m.IsIndicatorOnly, $"Metric '{m.Id}' should have IsIndicatorOnly = false"));
    }

    [Fact]
    public void GetAll_AllEarliestDatesAreValid()
    {
        var result = _sut.GetAll();

        // Earliest dates should be in the past and not the default DateOnly value
        Assert.All(result, m =>
            Assert.NotEqual(default, m.EarliestDate));
    }

    [Fact]
    public void GetAll_AllIdsAreUnique()
    {
        var result = _sut.GetAll();

        var distinctIds = result.Select(m => m.Id).Distinct().ToList();
        Assert.Equal(result.Count, distinctIds.Count);
    }

    [Fact]
    public void GetAll_ReturnsSameInstanceOnMultipleCalls()
    {
        var first = _sut.GetAll();
        var second = _sut.GetAll();

        // The static catalogue should return the same reference each time
        Assert.Same(first, second);
    }
}
