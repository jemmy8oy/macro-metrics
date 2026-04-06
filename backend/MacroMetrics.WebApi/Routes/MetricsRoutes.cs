using Bogus;

namespace MacroMetrics.WebApi.Routes;

public record DataPoint(string Date, double Value);

public static class MetricsRoutes
{

    private static readonly Dictionary<string, (string Label, string Unit)> IndicatorMeta = new()
    {
        ["cape"]             = ("CAPE Ratio",           "×"),
        ["uk-10yr-gilt"]     = ("UK 10yr Gilt Yield",   "%"),
        ["us-10yr-treasury"] = ("US 10yr Treasury Yield", "%"),
    };

    public static RouteGroupBuilder MapMetricsRoutes(this RouteGroupBuilder parentGroup)
    {
        var group = parentGroup.MapGroup("/metrics");

        // GET /api/metrics/ratio?numerator=X&denominator=Y
        group.MapGet("/ratio", (string numerator, string denominator) =>
        {
            var seed = (numerator + denominator).GetHashCode();
            var rng  = new Randomizer(Math.Abs(seed));

            var series  = GenerateSeries(rng, startValue: rng.Double(3, 12), volatility: 0.03);
            var longRun = series.Average(p => p.Value);

            return Results.Ok(new
            {
                numerator,
                denominator,
                longRunAverage = Math.Round(longRun, 2),
                series
            });
        })
        .WithName("GetRatio")
        .WithSummary("Returns the ratio time series for two comparable metrics.");

        // GET /api/metrics/indicator/{id}
        group.MapGet("/indicator/{id}", (string id) =>
        {
            if (!IndicatorMeta.TryGetValue(id, out var meta))
                return Results.NotFound(new { error = $"Unknown indicator '{id}'." });

            var seed = id.GetHashCode();
            var rng  = new Randomizer(Math.Abs(seed));

            double startValue  = id == "cape" ? rng.Double(14, 22) : rng.Double(2, 5);
            double volatility  = id == "cape" ? 0.04 : 0.05;

            var series  = GenerateSeries(rng, startValue, volatility);
            var longRun = series.Average(p => p.Value);

            return Results.Ok(new
            {
                id,
                label          = meta.Label,
                unit           = meta.Unit,
                longRunAverage = Math.Round(longRun, 2),
                series
            });
        })
        .WithName("GetIndicator")
        .WithSummary("Returns a time series for a standalone indicator (CAPE, gilt yield, treasury yield).");

        return parentGroup;
    }

    /// <summary>
    /// Generates ~30 years of monthly data (360 points) using a random walk.
    /// </summary>
    private static List<DataPoint> GenerateSeries(Randomizer rng, double startValue, double volatility)
    {
        var start  = new DateTime(1995, 1, 1);
        var points = new List<DataPoint>(360);
        var value  = startValue;

        for (int i = 0; i < 360; i++)
        {
            value = Math.Max(0.1, value * (1 + rng.Double(-volatility, volatility)));
            points.Add(new DataPoint(
                Date:  start.AddMonths(i).ToString("yyyy-MM-dd"),
                Value: Math.Round(value, 2)
            ));
        }

        return points;
    }
}
