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
        .WithName("GetMetrics")
        .WithSummary("Full metric catalogue with metadata.");

        group.MapGet("{id}", (string id, DateOnly? from, DateOnly? to, IMetricSeriesService seriesService) =>
        {
            var series = seriesService.GetSeries(id, from, to);

            if (series is null) return Results.NotFound();

            var response = new
            {
                id     = series.Id,
                label  = series.Label,
                unit   = series.Unit,
                points = series.Points.Select(p => new { date = p.Date, value = p.Value })
            };

            return Results.Ok(response);
        })
        .WithName("GetMetricSeries")
        .WithSummary("Time series for a single metric, with optional from/to date range filter.");

        return parentGroup;
    }
}
