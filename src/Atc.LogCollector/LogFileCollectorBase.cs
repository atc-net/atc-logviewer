// ReSharper disable InconsistentNaming
namespace Atc.LogCollector;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "OK.")]
public abstract class LogFileCollectorBase : LogCollectorBase
{
    internal const int MessageShortMaxLength = 160;

    internal readonly ConcurrentDictionary<string, TailFile> MonitoredFiles = new(StringComparer.Ordinal);

    internal static IList<FileInfo> GetFiles(
        DirectoryInfo directory,
        LogFileCollectorConfiguration config,
        string[] logExtensions)
    {
        var createdStartDate = config.MaxDaysBack == 0
            ? DateTime.Now.AddDays(-365)
            : DateTime.Now.AddDays(-config.MaxDaysBack);

        var files = new List<FileInfo>();

        foreach (var extension in logExtensions)
        {
            var searchPattern = $"*.{extension.TrimStart('.')}";
            if (config.FileNameTerms.Count == 0)
            {
                files.AddRange(directory.GetFiles(searchPattern));
            }
            else
            {
                var filesByExtension = directory.GetFiles(searchPattern);
                foreach (var file in filesByExtension)
                {
                    foreach (var term in config.FileNameTerms)
                    {
                        if (file.Name.Contains(term, StringComparison.OrdinalIgnoreCase) &&
                            !files.Contains(file))
                        {
                            files.Add(file);
                        }
                    }
                }
            }
        }

        return files
            .Where(f => f.CreationTime >= createdStartDate && f.CreationTime <= DateTime.Now)
            .OrderBy(x => x.CreationTime)
            .ToList();
    }

    public void MonitorFileIfNeeded(
        FileInfo fileInfo,
        long lastLineNumber)
    {
        if (!fileInfo.IsCreatedToday() ||
            MonitoredFiles.ContainsKey(fileInfo.FullName))
        {
            return;
        }

        var tailFile = new TailFile(fileInfo, lastLineNumber);
        tailFile.LineAdded += OnLineAdded;
        tailFile.Start();

        MonitoredFiles.TryAdd(fileInfo.FullName, tailFile);
    }

    public virtual void OnLineAdded(
        TailLine obj)
    {
    }

    public bool IsFileMonitoring(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return MonitoredFiles.ContainsKey(file.FullName);
    }

    public void StopMonitoringFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (!MonitoredFiles.TryRemove(file.FullName, out var monitoredFile))
        {
            return;
        }

        monitoredFile.Stop();
        monitoredFile.Dispose();
    }

    public void StopMonitoringAllFiles()
    {
        foreach (var monitoredFile in MonitoredFiles)
        {
            monitoredFile.Value.Stop();
            monitoredFile.Value.Dispose();
        }

        MonitoredFiles.Clear();
    }
}