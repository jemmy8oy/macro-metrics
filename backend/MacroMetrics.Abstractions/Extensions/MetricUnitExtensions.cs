using System.ComponentModel;
using System.Reflection;
using MacroMetrics.Abstractions.Enums;

namespace MacroMetrics.Abstractions.Extensions;

public static class MetricUnitExtensions
{
    /// <summary>
    /// Returns the human-readable display string for a <see cref="MetricUnit"/> value,
    /// as defined by the <see cref="DescriptionAttribute"/> on each enum member.
    /// </summary>
    public static string ToDisplayString(this MetricUnit unit)
    {
        var field = typeof(MetricUnit).GetField(unit.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? unit.ToString();
    }
}
