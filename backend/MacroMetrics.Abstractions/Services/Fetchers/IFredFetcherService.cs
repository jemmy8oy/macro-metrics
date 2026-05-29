using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.Abstractions.Services.Fetchers;

/// <summary>
/// Fetches raw time-series data from the FRED (Federal Reserve Economic Data) API.
/// Used for US macroeconomic metrics: us-house-prices, us-wages, us-cpi, cape, us-10yr-treasury.
/// </summary>
public interface IFredFetcherService
{
    Task<IReadOnlyList<IMetricPoint>> FetchRawAsync(string metricId);
}
