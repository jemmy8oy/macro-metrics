using MacroMetrics.Abstractions.Services;

namespace MacroMetrics.WebApi.Routes;

public static class MetricsRoutes
{
    public static RouteGroupBuilder MapMetricsRoutes(this RouteGroupBuilder parentGroup)
    {
        var group = parentGroup.MapGroup("/metrics");
        group.MapGet("/ratio",          GetRatio)     .WithName("GetRatio")     .WithSummary("Ratio time series for two comparable metrics.");
        group.MapGet("/indicator/{id}", GetIndicator) .WithName("GetIndicator") .WithSummary("Time series for a standalone indicator.");
        return parentGroup;
    }

    private static IResult GetRatio(string numerator, string denominator, IMetricsService svc)
        => Results.Ok(svc.GetRatio(numerator, denominator));

    private static IResult GetIndicator(string id, IMetricsService svc)
    {
        var result = svc.GetIndicator(id);
        return result is null
            ? Results.NotFound(new { error = $"Unknown indicator '{id}'." })
            : Results.Ok(result);
    }
}
