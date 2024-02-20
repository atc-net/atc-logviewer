// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.LogCollector.Serilog;

public class SerilogFileCollector : LogFileCollectorBase, ISerilogFileCollector
{
    private readonly ISerilogFileExtractor serilogFileExtractor;
    private readonly string[] defaultLogExtensions = ["log", "txt"];

    public SerilogFileCollector(
        ISerilogFileExtractor serilogFileExtractor)
    {
        ArgumentNullException.ThrowIfNull(serilogFileExtractor);

        this.serilogFileExtractor = serilogFileExtractor;
    }

    public event Action<AtcLogEntry>? CollectedEntry;

    public event Action<FileInfo>? CollectedFileDone;

    public event Action<FileInfo[]>? CollectedFilesDone;

    public async Task CollectAndMonitorFolder(
        DirectoryInfo directory,
        LogFileCollectorConfig config,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(config);

        var files = GetFiles(directory, config, defaultLogExtensions);
        if (files.Count == 0)
        {
            return;
        }

        foreach (var file in files.Select(x => x.FullName))
        {
            if (!MonitoredFiles.TryGetValue(file, out var oldTailFile))
            {
                continue;
            }

            oldTailFile.Stop();
            oldTailFile.Dispose();
            MonitoredFiles.TryRemove(file, out _);
        }

        var options = new ParallelOptions { CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(
            files,
            options,
            async (file, _) =>
        {
            var foundValidLogEntries = await ReadAndParseLines(
                file,
                cancellationToken).ConfigureAwait(false);

            if (!foundValidLogEntries)
            {
                return;
            }

            CollectedFileDone?.Invoke(file);

            if (!DateTime.Now.ToShortDateString().Equals(
                    file.CreationTime.ToShortDateString(),
                    StringComparison.Ordinal))
            {
                return;
            }

            var tailFile = new TailFile(file);
            tailFile.LineAdded += OnLineAdded;
            tailFile.Start();

            MonitoredFiles.TryAdd(file.FullName, tailFile);
        }).ConfigureAwait(false);

        CollectedFilesDone?.Invoke(files.ToArray());
    }

    public void MonitorFolder(
        DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        var files = directory.GetFiles("*.log")
            .Concat(directory.GetFiles("*.txt"))
            .OrderBy(x => x.CreationTime)
            .ToList();

        if (files.Count == 0)
        {
            return;
        }

        foreach (var file in files)
        {
            if (MonitoredFiles.TryGetValue(file.FullName, out var oldTailFile))
            {
                oldTailFile.Stop();
                oldTailFile.Dispose();
                MonitoredFiles.TryRemove(file.FullName, out _);
            }

            if (!DateTime.Now.ToShortDateString().Equals(file.CreationTime.ToShortDateString(), StringComparison.Ordinal))
            {
                continue;
            }

            var tailFile = new TailFile(file);
            tailFile.LineAdded += OnLineAdded;
            tailFile.Start();

            MonitoredFiles.TryAdd(file.FullName, tailFile);
        }
    }

    public void DeMonitorFolder(
        DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        foreach (var monitoredFile in MonitoredFiles.Keys)
        {
            var file = new FileInfo(monitoredFile);
            if (file.Directory!.FullName != directory.FullName)
            {
                continue;
            }

            if (!MonitoredFiles.TryGetValue(file.FullName, out var oldTailFile))
            {
                continue;
            }

            oldTailFile.Stop();
            oldTailFile.Dispose();
            MonitoredFiles.TryRemove(file.FullName, out _);
        }
    }

    public void DeMonitorAll()
    {
        foreach (var monitoredFile in MonitoredFiles.Keys)
        {
            var file = new FileInfo(monitoredFile);

            if (!MonitoredFiles.TryGetValue(file.FullName, out var oldTailFile))
            {
                continue;
            }

            oldTailFile.Stop();
            oldTailFile.Dispose();
            MonitoredFiles.TryRemove(file.FullName, out _);
        }
    }

    private void OnLineAdded(
        TailLine obj)
    {
        var sourceIdentifier = Path.GetFileNameWithoutExtension(obj.File.FullName);
        var logEntry = serilogFileExtractor.ParseRootLine(sourceIdentifier, obj.Line);

        // TODO: Handle subLines
        if (logEntry is not null)
        {
            CollectedEntry?.Invoke(logEntry);
        }
    }

    private async Task<bool> ReadAndParseLines(
        FileInfo file,
        CancellationToken cancellationToken)
    {
        var lines = await FileHelper.ReadAllTextToLinesAsync(
            file,
            cancellationToken).ConfigureAwait(false);

        if (lines.Length == 0)
        {
            return false;
        }

        var extractor = new SerilogFileExtractor();

        var sourceIdentifier = Path.GetFileNameWithoutExtension(file.FullName);
        var hasAnyValidLines = false;
        for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            var line = lines[lineNumber];
            var logEntry = extractor.ParseRootLine(sourceIdentifier, line);
            if (logEntry is null)
            {
                continue;
            }

            if (extractor.HasSubLines(lines, lineNumber))
            {
                var errorMessageWithSubLines = extractor.GetMessageWithSubLines(
                    logEntry.Message,
                    lines,
                    ref lineNumber);

                logEntry = logEntry with { Message = errorMessageWithSubLines };
            }

            CollectedEntry?.Invoke(logEntry);
            hasAnyValidLines = true;
        }

        return hasAnyValidLines;
    }
}