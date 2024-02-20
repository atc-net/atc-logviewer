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
        LogFileCollectorConfig config,
        string[] logExtensions)
    {
        var createdStartDate = DateTime.Now.AddDays(-config.MaxDaysBack);
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
}