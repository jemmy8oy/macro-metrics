using MacroMetrics.Abstractions.Services;
using MacroMetrics.Services;
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
        services.AddScoped<IMetricsService, MetricsService>();
    }
}
