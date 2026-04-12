using MacroMetrics.Abstractions.DataModels;

namespace MacroMetrics.DataModels.Models;

public record DataPointDto(string Date, double Value) : IDataPoint;
