namespace Atc.LogCollector;

public interface IFileCollector
{
    event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    Task CollectAndMonitorFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        CancellationToken cancellationToken);

    void StopMonitorFolder(
        DirectoryInfo directory);
}