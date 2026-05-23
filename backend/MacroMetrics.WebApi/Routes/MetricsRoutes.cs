using MacroMetrics.Abstractions.Extensions;
using MacroMetrics.Abstractions.Services.Metrics;

namespace MacroMetrics.WebApi.Routes;

public static class MetricsRoutes
{
    public static RouteGroupBuilder MapMetricsRoutes(this RouteGroupBuilder parentGroup)
    {
        var group = parentGroup.MapGroup("/metrics");

        group.MapGet("", (IMetricCatalogueService catalogueService) =>
        {
            var metrics = catalogueService.GetAll().Select(m => new
            {
                id = m.Id.ToDisplayString(),
                label = m.Label,
                unit = m.Unit.ToDisplayString(),
                source = m.Source.ToDisplayString(),
                isIndicatorOnly = m.IsIndicatorOnly,
                earliestDate = m.EarliestDate.ToString("yyyy-MM-dd")
            });

            return Results.Ok(metrics);
        })
        .WithName("GetMetrics");

        return parentGroup;
    }
}
