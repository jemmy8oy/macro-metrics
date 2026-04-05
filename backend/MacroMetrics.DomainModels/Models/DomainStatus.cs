using MacroMetrics.Abstractions.DomainModels;
using MacroMetrics.DataModels.Models;

namespace MacroMetrics.DomainModels.Models;

public class DomainStatus : Status, IDomainStatus
{
    public string GetFriendlyStatus()
    {
        return $"System is running version {Version} (Updated: {LastUpdated:g})";
    }
}
