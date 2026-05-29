using MacroMetrics.Abstractions.Services;
using MacroMetrics.Abstractions.Services.Fetchers;
using MacroMetrics.Abstractions.Services.Metrics;
using MacroMetrics.Abstractions.Services.Normalisation;
using MacroMetrics.Services;
using MacroMetrics.Services.Fetchers;
using MacroMetrics.Services.Metrics;
using MacroMetrics.Services.Normalisation;
using MacroMetrics.Database;
using Microsoft.EntityFrameworkCore;

namespace MacroMetrics.WebApi;

public static class ServiceRegistration
{
    public static void AddBackendServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("MacroMetrics.Database")));

        services.AddAutoMapper(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
        services.AddScoped<IStatusService, StatusService>();
        services.AddSingleton<IMetricCatalogueService, MetricCatalogueService>();
        services.AddScoped<IMetricSeriesService, MetricSeriesService>();
        services.AddScoped<IMetricRatioService, MetricRatioService>();
        services.AddScoped<IDataNormalisationService, DataNormalisationService>();

        // Fetcher services — stub implementations; real HTTP calls wired in future stories
        services.AddScoped<IOnsFetcherService, OnsFetcherService>();
        services.AddScoped<IFredFetcherService, FredFetcherService>();
        services.AddScoped<IYFinanceFetcherService, YFinanceFetcherService>();

        // Orchestrator — routes each metric to its correct fetcher by source
        services.AddScoped<IMetricSeriesOrchestrator, MetricSeriesOrchestrator>();
    }
}
