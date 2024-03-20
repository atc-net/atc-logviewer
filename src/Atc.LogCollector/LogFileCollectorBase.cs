// ReSharper disable InconsistentNaming
namespace Atc.LogCollector;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "OK.")]
[SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "OK.")]
public abstract class LogFileCollectorBase : LogCollectorBase
{
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
            files.AddRange(directory.GetFiles(searchPattern));
        }

        return files
            .Where(f => f.CreationTime >= createdStartDate && f.CreationTime <= DateTime.Now)
            .OrderBy(x => x.CreationTime)
            .ToList();
    }

    public void StopMonitorFolder(
        DirectoryInfo directory)
    {
        // TODO: Filter
        ////foreach (var monitoredFile in MonitoredFiles)
        ////{
        ////    monitoredFile.Value.Stop();
        ////    monitoredFile.Value.Dispose();
        ////}
    }
}