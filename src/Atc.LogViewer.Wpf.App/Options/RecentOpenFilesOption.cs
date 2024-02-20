namespace Atc.LogViewer.Wpf.App.Options;

public class RecentOpenFilesOption
{
    public IList<RecentOpenFileOption> RecentOpenFiles { get; init; } = new List<RecentOpenFileOption>();

    public override string ToString()
        => $"{nameof(RecentOpenFiles)}.Count: {RecentOpenFiles?.Count}";
}