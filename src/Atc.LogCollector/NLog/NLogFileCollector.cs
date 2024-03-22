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

    public Task CollectFile(
        FileInfo file,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task CollectFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();
}