namespace Atc.LogAnalyzer;

public interface ILogAnalyzer
{
    event Action<AtcLogEntry>? CollectedEntry;

    event Action<AtcLogEntry[]>? CollectedEntries;

    Task CollectFolder(
        LogFileCollectorType logFileCollectorType,
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken);

    public void SetFilter(
        LogFilter filter);

    public IDictionary<string, string> GetSourceSystems();

    AtcLogEntry[] GetFilteredLogEntries();

    LogStatistics GetLogStatistics();

    void ClearLogEntries();

    void StopMonitoringAndClearLogEntries();
}