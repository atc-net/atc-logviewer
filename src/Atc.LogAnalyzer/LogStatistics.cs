namespace Atc.LogAnalyzer;

public class LogStatistics
{
    public long Count { get; set; }

    public long CriticalCount { get; set; }

    public long ErrorCount { get; set; }

    public long WarningCount { get; set; }

    public long InformationCount { get; set; }

    public long DebugCount { get; set; }

    public long TraceCount { get; set; }

    public override string ToString()
        => $"{nameof(Count)}: {Count}, {nameof(CriticalCount)}: {CriticalCount}, {nameof(ErrorCount)}: {ErrorCount}, {nameof(WarningCount)}: {WarningCount}, {nameof(InformationCount)}: {InformationCount}, {nameof(DebugCount)}: {DebugCount}, {nameof(TraceCount)}: {TraceCount}";
}