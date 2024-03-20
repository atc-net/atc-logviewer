// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.LogViewer.Wpf.App.Options;

public class ProfileOptions
{
    public string Name { get; set; } = string.Empty;

    public string LogFolder { get; set; } = string.Empty;

    public LogFileCollectorType CollectorType { get; set; }

    public IList<HighlightOptions> Highlights { get; set; } = new List<HighlightOptions>();

    public LogFileCollectorConfiguration CollectorConfiguration { get; set; } = new();

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(LogFolder)}: {LogFolder}, {nameof(CollectorType)}: {CollectorType}, {nameof(Highlights)}.Count: {Highlights?.Count}";
}