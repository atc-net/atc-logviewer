namespace Atc.LogViewer.Wpf.App.Options;

public class HighlightOptions
{
    public string Text { get; set; } = string.Empty;

    public bool IgnoreCasing { get; set; }

    public string Foreground { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(Text)}: {Text}, {nameof(IgnoreCasing)}: {IgnoreCasing}, {nameof(Foreground)}: {Foreground}, {nameof(Background)}: {Background}";
}