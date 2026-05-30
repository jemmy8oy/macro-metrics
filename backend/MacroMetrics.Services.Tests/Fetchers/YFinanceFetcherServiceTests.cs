using System.Net;
using System.Text;
using MacroMetrics.Abstractions.Exceptions;
using MacroMetrics.Services.Fetchers;

namespace MacroMetrics.Services.Tests.Fetchers;

/// <summary>
/// Unit tests for <see cref="YFinanceFetcherService"/>.
/// HTTP calls are intercepted by <see cref="FakeHttpMessageHandler"/>.
/// </summary>
/// <remarks>
/// BDD-aligned from Issue #55 (US-B14).
/// </remarks>
public sealed class YFinanceFetcherServiceTests
{
    private const string SidecarBaseUrl = "http://yfinance-sidecar:8000";

    // ── Helpers ───────────────────────────────────────────────────────────

    private static YFinanceFetcherService BuildSut(FakeHttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri(SidecarBaseUrl) };
        return new YFinanceFetcherService(client);
    }

    /// <summary>Builds a minimal sidecar JSON payload.</summary>
    private static string BuildSidecarJson(string ticker, params (string Date, double Close)[] points)
    {
        var items = string.Join(",", points.Select(p =>
            $$$"""{"date":"{{{p.Date}}}","close":{{{p.Close.ToString(System.Globalization.CultureInfo.InvariantCulture)}}}]""".Replace("]", "}")));
        return $$"""{"ticker":"{{ticker}}","points":[{{items}}]}""";
    }

    private static HttpResponseMessage OkJson(string json)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

    // ── Scenario: YFinanceFetcherService maps metric IDs to correct tickers ──

    [Theory]
    [InlineData("gold",         "GC%3DF")]          // GC=F — = URL-encoded as %3D
    [InlineData("oil",          "CL%3DF")]          // CL=F — = URL-encoded as %3D
    [InlineData("ftse100",      "%5EFTSE")]          // ^FTSE — ^ URL-encoded as %5E
    [InlineData("sp500",        "%5EGSPC")]          // ^GSPC — ^ URL-encoded as %5E
    [InlineData("bitcoin",      "BTC-USD")]
    [InlineData("uk-10yr-gilt", "%5ETMBMKGB-10Y")]  // ^TMBMKGB-10Y — ^ URL-encoded as %5E
    public async Task FetchRawAsync_KnownMetric_CallsSidecarWithCorrectTicker(
        string metricId, string expectedEncodedTicker)
    {
        var handler = new FakeHttpMessageHandler(_ => OkJson(BuildSidecarJson("GC=F")));
        var sut = BuildSut(handler);

        await sut.FetchRawAsync(metricId);

        Assert.NotNull(handler.LastRequest);
        var path = handler.LastRequest!.RequestUri!.AbsolutePath;
        Assert.EndsWith(expectedEncodedTicker, path, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// BDD: When MetricSeriesOrchestrator.GetSeriesAsync("gold") is called,
    /// YFinanceFetcherService calls GET /series/GC=F on the sidecar.
    /// </summary>
    [Fact]
    public async Task FetchRawAsync_Gold_CallsSeriesGcFEndpoint()
    {
        var handler = new FakeHttpMessageHandler(_ => OkJson(BuildSidecarJson("GC=F")));
        var sut = BuildSut(handler);

        await sut.FetchRawAsync("gold");

        Assert.NotNull(handler.LastRequest);
        Assert.Contains("/series/", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Contains("GC%3DF", handler.LastRequest.RequestUri.AbsolutePath,
            StringComparison.OrdinalIgnoreCase);
    }

    // ── Scenario: Sidecar response shape is correctly consumed ────────────

    /// <summary>
    /// BDD: The sidecar returns a JSON body with "ticker" and a "points" array of
    /// {date, close} objects. Each close value is mapped to a DataPoint value,
    /// and each date string is preserved as-is (ISO-8601).
    /// </summary>
    [Fact]
    public async Task FetchRawAsync_ParsesPointsArrayIntoMetricPoints()
    {
        var json = BuildSidecarJson("GC=F",
            ("2024-01-15", 2023.50),
            ("2024-01-16", 2031.10));
        var handler = new FakeHttpMessageHandler(_ => OkJson(json));
        var sut = BuildSut(handler);

        var result = await sut.FetchRawAsync("gold");

        Assert.Equal(2, result.Count);

        Assert.Equal("2024-01-15", result[0].Date);
        Assert.Equal(2023.50, result[0].Value, precision: 5);

        Assert.Equal("2024-01-16", result[1].Date);
        Assert.Equal(2031.10, result[1].Value, precision: 5);
    }

    [Fact]
    public async Task FetchRawAsync_DatePassthrough_PreservesIsoFormat()
    {
        var json = BuildSidecarJson("BTC-USD",
            ("2014-09-30", 400.00),
            ("2024-01-01", 42000.00));
        var handler = new FakeHttpMessageHandler(_ => OkJson(json));
        var sut = BuildSut(handler);

        var result = await sut.FetchRawAsync("bitcoin");

        Assert.Equal("2014-09-30", result[0].Date);
        Assert.Equal("2024-01-01", result[1].Date);
    }

    [Fact]
    public async Task FetchRawAsync_EmptyPoints_ReturnsEmptyList()
    {
        var handler = new FakeHttpMessageHandler(_ => OkJson(BuildSidecarJson("^FTSE")));
        var sut = BuildSut(handler);

        var result = await sut.FetchRawAsync("ftse100");

        Assert.Empty(result);
    }

    // ── Error handling ────────────────────────────────────────────────────

    [Theory]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task FetchRawAsync_NonSuccessStatusCode_ThrowsFetcherException(
        HttpStatusCode statusCode)
    {
        var handler = new FakeHttpMessageHandler(
            _ => new HttpResponseMessage(statusCode));
        var sut = BuildSut(handler);

        var ex = await Assert.ThrowsAsync<FetcherException>(
            () => sut.FetchRawAsync("gold"));

        Assert.Contains("yfinance sidecar", ex.Message);
        Assert.Contains(((int)statusCode).ToString(), ex.Message);
    }

    [Fact]
    public async Task FetchRawAsync_UnknownMetricId_ThrowsArgumentException()
    {
        var handler = new FakeHttpMessageHandler(_ => OkJson("{}"));
        var sut = BuildSut(handler);

        await Assert.ThrowsAsync<ArgumentException>(
            () => sut.FetchRawAsync("unknown-metric"));
    }
}
