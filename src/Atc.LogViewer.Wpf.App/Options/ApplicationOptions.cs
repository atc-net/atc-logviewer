namespace Atc.LogViewer.Wpf.App.Options;

public class ApplicationOptions
{
    public const string SectionName = "Application";

    public string Theme { get; set; } = string.Empty;

    public bool OpenRecentProfileFileOnStartup { get; set; } = true;

    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(OpenRecentProfileFileOnStartup)}: {OpenRecentProfileFileOnStartup}";
}