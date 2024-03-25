// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.LogAnalyzer;

public sealed class LogAnalyzer : ILogAnalyzer, IDisposable
{
    private readonly ILog4NetFileCollector log4NetFileCollector;
    private readonly INLogFileCollector nlogFileCollector;
    private readonly ISerilogFileCollector serilogFileCollector;
    private readonly ConcurrentBag<AtcLogEntry> logEntries = [];
    private readonly ConcurrentBag<AtcLogEntry> logEntryBuffer = [];
    private FileSystemWatcher? fileSystemWatcher;
    private int logEntryBufferSize = 1;
    private LogFilter logFilter;
    private LogFileCollectorType activeLogFileCollectorType;
    private LogFileCollectorConfiguration? activeLogFileCollectorConfiguration;

    public LogAnalyzer(
        ILog4NetFileCollector log4NetFileCollector,
        INLogFileCollector nlogFileCollector,
        ISerilogFileCollector serilogFileCollector)
    {
        ArgumentNullException.ThrowIfNull(log4NetFileCollector);
        ArgumentNullException.ThrowIfNull(nlogFileCollector);
        ArgumentNullException.ThrowIfNull(serilogFileCollector);

        this.log4NetFileCollector = log4NetFileCollector;
        this.nlogFileCollector = nlogFileCollector;
        this.serilogFileCollector = serilogFileCollector;

        logFilter = new LogFilter(
            LogLevelTrace: true,
            LogLevelDebug: true,
            LogLevelInfo: true,
            LogLevelWarning: true,
            LogLevelError: true,
            LogLevelCritical: true,
            IncludeText: string.Empty,
            DateTimeFrom: null,
            DateTimeTo: null,
            SourceSystem: string.Empty);

        this.log4NetFileCollector.CollectedEntry += OnCollectedEntry;
        this.log4NetFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.log4NetFileCollector.CollectedFilesDone += OnCollectedFilesDone;

        this.nlogFileCollector.CollectedEntry += OnCollectedEntry;
        this.nlogFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.nlogFileCollector.CollectedFilesDone += OnCollectedFilesDone;

        this.serilogFileCollector.CollectedEntry += OnCollectedEntry;
        this.serilogFileCollector.CollectedFileDone += OnCollectedFileDone;
        this.serilogFileCollector.CollectedFilesDone += OnCollectedFilesDone;
    }

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<AtcLogEntry[]>? CollectedEntries;

    public LogFileCollectorType DetermineCollectorType(
        DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var files = Directory
            .GetFiles(directory.FullName)
            .Select(x => new FileInfo(x))
            .OrderByDescending(x => x.CreationTime);

        return files
            .Select(DetermineCollectorType)
            .FirstOrDefault(x => x != LogFileCollectorType.None);
    }

    public LogFileCollectorType DetermineCollectorType(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (serilogFileCollector.CanParseFileFormat(file))
        {
            return LogFileCollectorType.Serilog;
        }

        if (nlogFileCollector.CanParseFileFormat(file))
        {
            return LogFileCollectorType.NLog;
        }

        if (log4NetFileCollector.CanParseFileFormat(file))
        {
            return LogFileCollectorType.Log4Net;
        }

        return LogFileCollectorType.None;
    }

    public async Task CollectFile(
        LogFileCollectorType logFileCollectorType,
        FileInfo file,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(config);

        logEntryBufferSize = 10_000_000;

        if (fileSystemWatcher is not null)
        {
            fileSystemWatcher.Created -= OnFileCreated;
            fileSystemWatcher.Dispose();
        }

        activeLogFileCollectorType = logFileCollectorType;
        activeLogFileCollectorConfiguration = config;

        switch (logFileCollectorType)
        {
            case LogFileCollectorType.Log4Net:
                await log4NetFileCollector
                    .CollectFile(
                        file,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(false);
                break;
            case LogFileCollectorType.NLog:
                await nlogFileCollector
                    .CollectFile(
                        file,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(false);
                break;
            case LogFileCollectorType.Serilog:
                await serilogFileCollector
                    .CollectFile(
                        file,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(false);
                break;
            default:
                throw new SwitchCaseDefaultException(activeLogFileCollectorType);
        }

        logEntryBufferSize = 1;
        NotifyAndClearBuffer();

        fileSystemWatcher = new FileSystemWatcher(file.Directory!.FullName) { EnableRaisingEvents = true, };
        fileSystemWatcher.Created += OnFileCreated;
    }

    public async Task CollectFolder(
        LogFileCollectorType logFileCollectorType,
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(config);

        logEntryBufferSize = 10_000_000;

        if (fileSystemWatcher is not null)
        {
            fileSystemWatcher.Created -= OnFileCreated;
            fileSystemWatcher.Dispose();
        }

        activeLogFileCollectorType = logFileCollectorType;
        activeLogFileCollectorConfiguration = config;

        switch (activeLogFileCollectorType)
        {
            case LogFileCollectorType.Log4Net:
                await log4NetFileCollector
                    .CollectFolder(
                        directory,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
                break;
            case LogFileCollectorType.NLog:
                await nlogFileCollector
                    .CollectFolder(
                        directory,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
                break;
            case LogFileCollectorType.Serilog:
                await serilogFileCollector
                    .CollectFolder(
                        directory,
                        config,
                        useMonitoring,
                        cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
                break;
            default:
                throw new SwitchCaseDefaultException(logFileCollectorType);
        }

        logEntryBufferSize = 1;
        NotifyAndClearBuffer();

        fileSystemWatcher = new FileSystemWatcher(directory.FullName) { EnableRaisingEvents = true, };
        fileSystemWatcher.Created += OnFileCreated;
    }

    public void SetFilter(
        LogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        logFilter = filter;
    }

    public IDictionary<string, string> GetSourceSystems()
    {
        var sourceSystems = logEntries
            .GroupBy(x => x.SourceSystem, StringComparer.Ordinal)
            .Select(x => x.Key)
            .ToList();

        sourceSystems = DropDownFirstItemTypeHelper.EnsureFirstItemType(sourceSystems, DropDownFirstItemType.Blank);

        return sourceSystems.ToDictionary(
            x => x,
            x => x,
            StringComparer.Ordinal);
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
                         (!logFilter.DateTimeFrom.HasValue || entry.TimeStamp >= logFilter.DateTimeFrom.Value)
                         &&
                         (!logFilter.DateTimeTo.HasValue || entry.TimeStamp <= logFilter.DateTimeTo.Value)
                         &&
                         (logFilter.IncludeText.Length == 0 ||
                          entry.MessageFull.Contains(logFilter.IncludeText, StringComparison.OrdinalIgnoreCase))
                         &&
                         (logFilter.SourceSystem.Length == 0 ||
                          entry.SourceSystem.Equals(logFilter.SourceSystem, StringComparison.Ordinal)))
            .OrderBy(x => x.TimeStamp)
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

    public void StopMonitoringAndClearLogEntries()
    {
        log4NetFileCollector.StopMonitoringAllFiles();
        nlogFileCollector.StopMonitoringAllFiles();
        serilogFileCollector.StopMonitoringAllFiles();
        ClearLogEntries();
    }

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
            !logEntry.MessageFull.Contains(logFilter.IncludeText, StringComparison.OrdinalIgnoreCase))
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

    private void OnFileCreated(
        object sender,
        FileSystemEventArgs e)
    {
        if (activeLogFileCollectorConfiguration is null)
        {
            return;
        }

        var fileInfo = new FileInfo(e.FullPath);

        TaskHelper.FireAndForget(
            CollectFile(
                activeLogFileCollectorType,
                fileInfo,
                activeLogFileCollectorConfiguration,
                useMonitoring: true,
                CancellationToken.None));
    }

    private void NotifyAndClearBuffer()
    {
        if (CollectedEntries is not null)
        {
            var entries = logEntryBuffer
                .OrderBy(x => x.TimeStamp)
                .ToArray();

            CollectedEntries.Invoke(entries);
        }

        logEntryBuffer.Clear();
    }

    public void Dispose()
        => fileSystemWatcher?.Dispose();
}