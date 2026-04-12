using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.Services;
using MacroMetrics.DataModels.Models;
using Microsoft.AspNetCore.Http.HttpResults;

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

    private static Ok<Ratio> GetRatio(string numerator, string denominator, IMetricsService svc)
        => TypedResults.Ok(ToModel(svc.GetRatio(numerator, denominator)));

    private static Results<Ok<Indicator>, NotFound> GetIndicator(string id, IMetricsService svc)
    {
        var indicator = svc.GetIndicator(id);
        return indicator is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ToModel(indicator));
    }

    private static Ratio ToModel(IRatio r) => new()
    {
        Numerator      = r.Numerator,
        Denominator    = r.Denominator,
        LongRunAverage = r.LongRunAverage,
        Series         = r.Series.Select(p => new DataPoint { Date = p.Date, Value = p.Value }).ToList()
    };

    private static Indicator ToModel(IIndicator i) => new()
    {
        Id             = i.Id,
        Label          = i.Label,
        Unit           = i.Unit,
        LongRunAverage = i.LongRunAverage,
        Series         = i.Series.Select(p => new DataPoint { Date = p.Date, Value = p.Value }).ToList()
    };
}
