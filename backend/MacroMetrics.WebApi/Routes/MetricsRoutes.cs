using AutoMapper;
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

    private static Ok<Ratio> GetRatio(string numerator, string denominator, IMetricsService svc, IMapper mapper)
        => TypedResults.Ok(mapper.Map<Ratio>(svc.GetRatio(numerator, denominator)));

    private static Results<Ok<Indicator>, NotFound> GetIndicator(string id, IMetricsService svc, IMapper mapper)
    {
        var indicator = svc.GetIndicator(id);
        return indicator is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(mapper.Map<Indicator>(indicator));
    }
}
