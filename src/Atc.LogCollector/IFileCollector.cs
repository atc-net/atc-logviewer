namespace Atc.LogCollector;

public interface IFileCollector
{
    event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    Task CollectFile(
        FileInfo file,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken);

    Task CollectFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken);

    void MonitorFileIfNeeded(
        FileInfo fileInfo,
        long lastLineNumber = 0);

    void StopMonitoringAllFiles();

    void StopMonitoringFile(FileInfo file);
}