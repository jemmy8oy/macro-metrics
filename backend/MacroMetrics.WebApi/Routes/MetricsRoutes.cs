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

    private static Ok<RatioResponse> GetRatio(string numerator, string denominator, IMetricsService svc)
        => TypedResults.Ok(ToResponse(svc.GetRatio(numerator, denominator)));

    private static Results<Ok<IndicatorResponse>, NotFound> GetIndicator(string id, IMetricsService svc)
    {
        var indicator = svc.GetIndicator(id);
        return indicator is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ToResponse(indicator));
    }

    private static RatioResponse ToResponse(IRatio r) =>
        new(r.Numerator, r.Denominator, r.LongRunAverage,
            r.Series.Select(p => new DataPointDto(p.Date, p.Value)).ToList());

    private static IndicatorResponse ToResponse(IIndicator i) =>
        new(i.Id, i.Label, i.Unit, i.LongRunAverage,
            i.Series.Select(p => new DataPointDto(p.Date, p.Value)).ToList());
}
