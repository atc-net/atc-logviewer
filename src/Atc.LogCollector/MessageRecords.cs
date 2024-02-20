#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

namespace Atc.LogCollector;

public record LogEntriesStatsMessage(
    long Count,
    long TotalCount,
    long CriticalCount,
    long TotalCriticalCount,
    long ErrorCount,
    long TotalErrorCount,
    long WarningCount,
    long TotalWarningCount,
    long InformationCount,
    long TotalInformationCount,
    long DebugCount,
    long TotalDebugCount,
    long TraceCount,
    long TotalTraceCount);

#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore MA0048 // File name must match type name