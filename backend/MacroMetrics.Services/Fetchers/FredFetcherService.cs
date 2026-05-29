using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services.Fetchers;

namespace MacroMetrics.Services.Fetchers;

/// <summary>
/// Stub implementation of <see cref="IFredFetcherService"/>.
/// Real HTTP calls to the FRED API will be wired in a future story.
/// </summary>
public class FredFetcherService : IFredFetcherService
{
    public Task<IReadOnlyList<IMetricPoint>> FetchRawAsync(string metricId)
        => Task.FromResult<IReadOnlyList<IMetricPoint>>(Array.Empty<IMetricPoint>());
}
