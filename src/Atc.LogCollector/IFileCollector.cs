namespace Atc.LogCollector;

public interface IFileCollector
{
    event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    Task CollectFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken);

    void StopMonitoring();
}