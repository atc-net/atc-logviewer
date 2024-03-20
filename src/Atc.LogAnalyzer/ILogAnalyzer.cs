namespace Atc.LogAnalyzer;

public interface ILogAnalyzer
{
    event Action<AtcLogEntry>? CollectedEntry;

    event Action<AtcLogEntry[]>? CollectedEntries;

    Task CollectAndMonitorFolder(
        LogFileCollectorType logFileCollectorType,
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        CancellationToken cancellationToken);

    public void SetFilter(
        LogFilter filter);

    public IDictionary<string, string> GetSourceSystems();

    AtcLogEntry[] GetFilteredLogEntries();

    LogStatistics GetLogStatistics();

    void ClearLogEntries();

    void StopMonitorFolderAndClearLogEntries(
        DirectoryInfo directory);
}