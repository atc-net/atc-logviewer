namespace Atc.LogCollector.Log4Net;

public class Log4NetFileCollector : LogFileCollectorBase, ILog4NetFileCollector
{
    private readonly ILog4NetFileExtractor log4NetFileExtractor;
    private readonly string[] defaultLogExtensions = ["log"];

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    public Log4NetFileCollector(
        ILog4NetFileExtractor log4NetFileExtractor)
    {
        ArgumentNullException.ThrowIfNull(log4NetFileExtractor);

        this.log4NetFileExtractor = log4NetFileExtractor;
    }

    public Task CollectAndMonitorFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();
}