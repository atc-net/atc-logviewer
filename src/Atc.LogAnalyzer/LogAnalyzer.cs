// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable InvertIf
namespace Atc.LogAnalyzer;

public sealed class LogAnalyzer : ILogAnalyzer
{
    private readonly ILog4NetFileCollector log4NetFileCollector;
    private readonly INLogFileCollector nlogFileCollector;
    private readonly ISerilogFileCollector serilogFileCollector;
    private readonly ISyslogFileCollector syslogFileCollector;
    private readonly ConcurrentBag<AtcLogEntry> logEntries = new();
    private readonly ConcurrentBag<AtcLogEntry> logEntryBuffer = new();
    private int logEntryBufferSize = 1;
    private LogFilter logFilter;

    public LogAnalyzer(
        ILog4NetFileCollector log4NetFileCollector,
        INLogFileCollector nlogFileCollector,
        ISerilogFileCollector serilogFileCollector,
        ISyslogFileCollector syslogFileCollector)
    {
        ArgumentNullException.ThrowIfNull(log4NetFileCollector);
        ArgumentNullException.ThrowIfNull(nlogFileCollector);
        ArgumentNullException.ThrowIfNull(serilogFileCollector);
        ArgumentNullException.ThrowIfNull(syslogFileCollector);

        this.log4NetFileCollector = log4NetFileCollector;
        this.nlogFileCollector = nlogFileCollector;
        this.serilogFileCollector = serilogFileCollector;
        this.syslogFileCollector = syslogFileCollector;

        logFilter = new LogFilter(
            LogLevelTrace: true,
            LogLevelDebug: true,
            LogLevelInfo: true,
            LogLevelWarning: true,
            LogLevelError: true,
            LogLevelCritical: true,
            IncludeText: string.Empty);

        this.log4NetFileCollector.CollectedEntry += OnCollectedEntry;
        this.log4NetFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.log4NetFileCollector.CollectedFilesDone += OnCollectedFilesDone;

        this.nlogFileCollector.CollectedEntry += OnCollectedEntry;
        this.nlogFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.nlogFileCollector.CollectedFilesDone += OnCollectedFilesDone;

        this.serilogFileCollector.CollectedEntry += OnCollectedEntry;
        this.serilogFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.serilogFileCollector.CollectedFilesDone += OnCollectedFilesDone;

        this.syslogFileCollector.CollectedEntry += OnCollectedEntry;
        this.syslogFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.syslogFileCollector.CollectedFilesDone += OnCollectedFilesDone;
    }

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<AtcLogEntry[]>? CollectedEntries;

    public async Task CollectAndMonitorFolder(
        LogFileCollectorType logFileCollectorType,
        DirectoryInfo directory,
        LogFileCollectorConfig config,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directory);

        logEntryBufferSize = 10_000_000;

        switch (logFileCollectorType)
        {
            case LogFileCollectorType.Log4Net:
                CollectAndMonitorFolderForLog4Net(directory, cancellationToken);
                break;
            case LogFileCollectorType.NLog:
                CollectAndMonitorFolderForNLog(directory, cancellationToken);
                break;
            case LogFileCollectorType.Serilog:
                await CollectAndMonitorFolderForSerilog(directory, config, cancellationToken).ConfigureAwait(false);
                break;
            case LogFileCollectorType.Syslog:
                CollectAndMonitorFolderForSyslog(directory, cancellationToken);
                break;
            default:
                throw new SwitchCaseDefaultException(logFileCollectorType);
        }

        logEntryBufferSize = 1;
        NotifyAndClearBuffer();
    }

    public void SetFilter(
        LogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        logFilter = filter;
    }

    public AtcLogEntry[] GetFilteredLogEntries()
        => logEntries
            .Where(
                entry => ((entry.LogLevel == LogLevel.Trace && logFilter.LogLevelTrace) ||
                          (entry.LogLevel == LogLevel.Debug && logFilter.LogLevelDebug) ||
                          (entry.LogLevel == LogLevel.Information && logFilter.LogLevelInfo) ||
                          (entry.LogLevel == LogLevel.Warning && logFilter.LogLevelWarning) ||
                          (entry.LogLevel == LogLevel.Error && logFilter.LogLevelError) ||
                          (entry.LogLevel == LogLevel.Critical && logFilter.LogLevelCritical))
                         &&
                         (logFilter.IncludeText.Length == 0 ||
                          entry.Message.Contains(logFilter.IncludeText, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

    public LogStatistics GetLogStatistics()
        => new()
        {
            Count = logEntries.Count,
            CriticalCount = logEntries.Count(x => x.LogLevel == LogLevel.Critical),
            ErrorCount = logEntries.Count(x => x.LogLevel == LogLevel.Error),
            WarningCount = logEntries.Count(x => x.LogLevel == LogLevel.Warning),
            InformationCount = logEntries.Count(x => x.LogLevel == LogLevel.Information),
            DebugCount = logEntries.Count(x => x.LogLevel == LogLevel.Debug),
            TraceCount = logEntries.Count(x => x.LogLevel == LogLevel.Trace),
        };

    public void ClearLogEntries()
        => logEntries.Clear();

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void OnCollectedEntry(
        AtcLogEntry logEntry)
    {
        logEntries.Add(logEntry);

        switch (logEntry.LogLevel)
        {
            case LogLevel.Trace:
                if (!logFilter.LogLevelTrace)
                {
                    return;
                }

                break;
            case LogLevel.Debug:
                if (!logFilter.LogLevelDebug)
                {
                    return;
                }

                break;
            case LogLevel.Information:
                if (!logFilter.LogLevelInfo)
                {
                    return;
                }

                break;
            case LogLevel.Warning:
                if (!logFilter.LogLevelWarning)
                {
                    return;
                }

                break;
            case LogLevel.Error:
                if (!logFilter.LogLevelError)
                {
                    return;
                }

                break;
            case LogLevel.Critical:
                if (!logFilter.LogLevelCritical)
                {
                    return;
                }

                break;
        }

        if (logFilter.IncludeText.Length > 0 &&
            !logEntry.Message.Contains(logFilter.IncludeText, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        logEntryBuffer.Add(logEntry);

        if (logEntryBuffer.Count >= logEntryBufferSize)
        {
            NotifyAndClearBuffer();
        }
    }

    private void OnCollectedFileDone(
        FileInfo file)
    {
        // Dummy
    }

    private void OnCollectedFilesDone(
        FileInfo[] files)
        => NotifyAndClearBuffer();

    private void NotifyAndClearBuffer()
    {
        if (CollectedEntries is not null)
        {
            var entries = logEntryBuffer
                .OrderBy(x => x.DateTime)
                .ToArray();

            CollectedEntries.Invoke(entries);
        }

        logEntryBuffer.Clear();
    }

    private void CollectAndMonitorFolderForLog4Net(
        DirectoryInfo directory,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void CollectAndMonitorFolderForNLog(
        DirectoryInfo directory,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task CollectAndMonitorFolderForSerilog(
        DirectoryInfo directory,
        LogFileCollectorConfig config,
        CancellationToken cancellationToken)
        => serilogFileCollector.CollectAndMonitorFolder(directory, config, cancellationToken);

    private void CollectAndMonitorFolderForSyslog(
        DirectoryInfo directory,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}