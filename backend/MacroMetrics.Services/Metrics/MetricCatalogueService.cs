using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Enums;
using MacroMetrics.Abstractions.Services.Metrics;
using MacroMetrics.DataModels.Models;

namespace MacroMetrics.Services.Metrics;

public class MetricCatalogueService : IMetricCatalogueService
{
    private static readonly IReadOnlyList<IMetricMetadata> Catalogue = new List<IMetricMetadata>
    {
        // yfinance metrics
        new MetricMetadata
        {
            Id = "gold",
            Label = "Gold Price",
            Unit = "$/oz",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1979, 1, 31)
        },
        new MetricMetadata
        {
            Id = "oil",
            Label = "Oil Price (WTI)",
            Unit = "$/barrel",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1983, 3, 31)
        },
        new MetricMetadata
        {
            Id = "ftse100",
            Label = "FTSE 100",
            Unit = "Index (GBP)",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1984, 1, 31)
        },
        new MetricMetadata
        {
            Id = "sp500",
            Label = "S&P 500",
            Unit = "Index (USD)",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1950, 1, 31)
        },
        new MetricMetadata
        {
            Id = "bitcoin",
            Label = "Bitcoin",
            Unit = "USD",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(2014, 9, 30)
        },
        new MetricMetadata
        {
            Id = "uk-10yr-gilt",
            Label = "UK 10-Year Gilt Yield",
            Unit = "% Yield",
            Source = MetricSource.YFinance,
            IsIndicatorOnly = true,
            EarliestDate = new DateOnly(1990, 1, 31)
        },

        // FRED metrics
        new MetricMetadata
        {
            Id = "us-house-prices",
            Label = "US House Prices",
            Unit = "Index",
            Source = MetricSource.Fred,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1987, 1, 31)
        },
        new MetricMetadata
        {
            Id = "us-wages",
            Label = "US Wages",
            Unit = "$/hour",
            Source = MetricSource.Fred,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1964, 3, 31)
        },
        new MetricMetadata
        {
            Id = "us-cpi",
            Label = "US CPI",
            Unit = "Index",
            Source = MetricSource.Fred,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1947, 1, 31)
        },
        new MetricMetadata
        {
            Id = "cape",
            Label = "Shiller CAPE Ratio",
            Unit = "Ratio",
            Source = MetricSource.Fred,
            IsIndicatorOnly = true,
            EarliestDate = new DateOnly(1881, 1, 31)
        },
        new MetricMetadata
        {
            Id = "us-10yr-treasury",
            Label = "US 10-Year Treasury Yield",
            Unit = "% Yield",
            Source = MetricSource.Fred,
            IsIndicatorOnly = true,
            EarliestDate = new DateOnly(1962, 1, 31)
        },

        // ONS metrics
        new MetricMetadata
        {
            Id = "uk-house-prices",
            Label = "UK House Prices",
            Unit = "Index (£)",
            Source = MetricSource.OnsHpi,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1968, 1, 31)
        },
        new MetricMetadata
        {
            Id = "uk-wages",
            Label = "UK Wages",
            Unit = "£/month",
            Source = MetricSource.OnsAwe,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1963, 1, 31)
        },
        new MetricMetadata
        {
            Id = "uk-cpi",
            Label = "UK CPI",
            Unit = "Index",
            Source = MetricSource.Ons,
            IsIndicatorOnly = false,
            EarliestDate = new DateOnly(1988, 1, 31)
        }
    };

    public IReadOnlyList<IMetricMetadata> GetAll() => Catalogue;
}
