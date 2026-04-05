using MacroMetrics.Abstractions.DataModels;
using MacroMetrics.Abstractions.DomainModels;

namespace MacroMetrics.Abstractions.Services;

public interface IStatusService
{
    Task<IDomainStatus> GetSystemStatusAsync();
}
