using System.ComponentModel;
using System.Reflection;
using MacroMetrics.Abstractions.Enums;

namespace MacroMetrics.Abstractions.Extensions;

public static class MetricSourceExtensions
{
    /// <summary>
    /// Returns the human-readable display string for a <see cref="MetricSource"/> value,
    /// as defined by the <see cref="DescriptionAttribute"/> on each enum member.
    /// </summary>
    public static string ToDisplayString(this MetricSource source)
    {
        var field = typeof(MetricSource).GetField(source.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? source.ToString();
    }
}
