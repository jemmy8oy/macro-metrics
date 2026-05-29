using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services.Fetchers;

namespace MacroMetrics.Services.Fetchers;

/// <summary>
/// Stub implementation of <see cref="IYFinanceFetcherService"/>.
/// Real HTTP calls to the Yahoo Finance sidecar will be wired in a future story.
/// </summary>
public class YFinanceFetcherService : IYFinanceFetcherService
{
    public Task<IReadOnlyList<IMetricPoint>> FetchRawAsync(string metricId)
        => Task.FromResult<IReadOnlyList<IMetricPoint>>(Array.Empty<IMetricPoint>());
}
