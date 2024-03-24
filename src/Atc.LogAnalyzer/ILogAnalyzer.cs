namespace Atc.LogAnalyzer;

public interface ILogAnalyzer
{
    event Action<AtcLogEntry>? CollectedEntry;

    event Action<AtcLogEntry[]>? CollectedEntries;

    LogFileCollectorType DetermineCollectorType(
        DirectoryInfo directory);

    LogFileCollectorType DetermineCollectorType(
        FileInfo file);

    Task CollectFile(
        LogFileCollectorType logFileCollectorType,
        FileInfo file,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken);

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