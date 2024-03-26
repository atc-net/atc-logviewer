namespace Atc.LogCollector.Log4Net;

public class Log4NetFileCollector : LogFileCollectorBase, ILog4NetFileCollector
{
    private readonly ILog4NetFileExtractor fileExtractor;
    private readonly string[] defaultLogExtensions = ["log"];

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    public Log4NetFileCollector(
        ILog4NetFileExtractor log4NetFileExtractor)
    {
        ArgumentNullException.ThrowIfNull(log4NetFileExtractor);

        fileExtractor = log4NetFileExtractor;
    }

    public bool CanParseFileFormat(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (!defaultLogExtensions.Contains(
                file.Extension.Replace(".", string.Empty, StringComparison.Ordinal),
                StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            using var sr = new StreamReader(file.FullName);
            for (var i = 0; i < 3; i++)
            {
                if (sr.EndOfStream)
                {
                    break;
                }

                var line = sr.ReadLine();
                if (line is null)
                {
                    return false;
                }

                if (fileExtractor.ParseRootLine(string.Empty, string.Empty, 0, line) is not null)
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    public async Task CollectFile(
        FileInfo file,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(config);

        if (IsFileMonitoring(file))
        {
            return;
        }

        if (!CanParseFileFormat(file))
        {
            return;
        }

        var (isSuccessFul, lastLineNumber) = await ReadAndParseLines(
                file,
                cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        if (!isSuccessFul)
        {
            return;
        }

        CollectedFileDone?.Invoke(file);

        if (useMonitoring)
        {
            MonitorFileIfNeeded(file, lastLineNumber);
        }

        CollectedFilesDone?.Invoke([file]);
    }

    public async Task CollectFolder(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        bool useMonitoring,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(config);

        var files = GetFiles(directory, config, defaultLogExtensions);
        if (files.Count == 0)
        {
            return;
        }

        StopMonitoringAllFiles();

        var options = new ParallelOptions { CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(
            files,
            options,
            async (file, _) =>
            {
                if (!CanParseFileFormat(file))
                {
                    return;
                }

                var (isSuccessFul, lastLineNumber) = await ReadAndParseLines(
                        file,
                        cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

                if (!isSuccessFul)
                {
                    return;
                }

                CollectedFileDone?.Invoke(file);

                if (useMonitoring)
                {
                    MonitorFileIfNeeded(file, lastLineNumber);
                }
            }).ConfigureAwait(continueOnCapturedContext: false);

        CollectedFilesDone?.Invoke([.. files]);
    }

    internal async Task<(bool IsSuccessFul, long LastLineNumber)> ReadAndParseLines(
        FileInfo file,
        CancellationToken cancellationToken)
    {
        var lines = await FileHelper.ReadAllTextToLinesAsync(
                file,
                cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        if (lines.Length == 0)
        {
            return (false, 0);
        }

        var extractor = new Log4NetFileExtractor();

        var sourceIdentifier = Path.GetFileNameWithoutExtension(file.FullName);
        var sourceSystem = GetSourceSystemFromSourceIdentifier(sourceIdentifier);
        var hasAnyValidLines = false;
        for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            var line = lines[lineNumber];
            var logEntry = extractor.ParseRootLine(sourceIdentifier, sourceSystem, lineNumber + 1, line);
            if (logEntry is null)
            {
                continue;
            }

            if (extractor.HasSubLines(lines, lineNumber))
            {
                var errorMessageWithSubLines = extractor.GetMessageWithSubLines(
                    logEntry.MessageShort,
                    lines,
                    ref lineNumber);

                logEntry = logEntry with { MessageFull = errorMessageWithSubLines };

                if (logEntry.MessageFull.IsFormatJson())
                {
                    logEntry = logEntry with { MessageShort = "[JSON]" };
                }
            }

            if (logEntry.MessageShort.Length > MessageShortMaxLength)
            {
                var cutMessage = string.Concat(logEntry.MessageShort.AsSpan(0, MessageShortMaxLength), "...");
                logEntry = logEntry with { MessageShort = cutMessage };
            }

            CollectedEntry?.Invoke(logEntry);
            hasAnyValidLines = true;
        }

        var numberOfLines = lines.Length;
        if (numberOfLines > 0 &&
            string.IsNullOrEmpty(lines[^1]))
        {
            numberOfLines--;
        }

        return (hasAnyValidLines, numberOfLines);
    }
}