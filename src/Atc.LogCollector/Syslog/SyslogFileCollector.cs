namespace Atc.LogCollector.Syslog;

public class SyslogFileCollector : LogFileCollectorBase, ISyslogFileCollector
{
    private readonly ISyslogFileExtractor syslogFileExtractor;
    private readonly string[] defaultLogExtensions = ["log"];

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    public SyslogFileCollector(
        ISyslogFileExtractor syslogFileExtractor)
    {
        ArgumentNullException.ThrowIfNull(syslogFileExtractor);

        this.syslogFileExtractor = syslogFileExtractor;
    }

    public Task CollectFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();
}