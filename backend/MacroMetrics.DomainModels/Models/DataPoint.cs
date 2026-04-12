using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DomainModels.Models;

public record DataPoint(string Date, double Value) : IDataPoint;
