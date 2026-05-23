using System.ComponentModel;
using System.Reflection;
using MacroMetrics.Abstractions.Enums;

namespace MacroMetrics.Abstractions.Extensions;

public static class MetricIdExtensions
{
    /// <summary>
    /// Returns the kebab-case display string for a <see cref="MetricId"/> value,
    /// as defined by the <see cref="DescriptionAttribute"/> on each enum member.
    /// </summary>
    public static string ToDisplayString(this MetricId metricId)
    {
        var field = typeof(MetricId).GetField(metricId.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? metricId.ToString();
    }
}
