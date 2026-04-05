namespace MacroMetrics.Abstractions.DomainModels;

using MacroMetrics.Abstractions.DataModels;

public interface IDomainStatus : IStatus
{
    string GetFriendlyStatus();
}
