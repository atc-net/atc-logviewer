namespace Atc.LogCollector.NLog;

public class NLogFileCollector : LogFileCollectorBase, INLogFileCollector
{
    private readonly INLogFileExtractor nlogFileExtractor;
    private readonly string[] defaultLogExtensions = ["log"];

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    public NLogFileCollector(
        INLogFileExtractor nlogFileExtractor)
    {
        ArgumentNullException.ThrowIfNull(nlogFileExtractor);

        this.nlogFileExtractor = nlogFileExtractor;
    }

    public Task CollectAndMonitorFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();
}