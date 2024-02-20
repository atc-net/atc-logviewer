namespace Atc.LogViewer.Wpf.App.Options;

public class RecentOpenFileOption
{
    public DateTime TimeStamp { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(FilePath)}: {FilePath}";
}