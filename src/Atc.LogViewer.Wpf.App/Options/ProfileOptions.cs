// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.LogViewer.Wpf.App.Options;

public class ProfileOptions
{
    public string DefaultLogFolder { get; set; } = string.Empty;

    public LogFileCollectorType DefaultCollectorType { get; set; }

    public IList<HighlightOptions> Highlights { get; set; } = new List<HighlightOptions>();

    public override string ToString()
        => $"{nameof(DefaultLogFolder)}: {DefaultLogFolder}, {nameof(DefaultCollectorType)}: {DefaultCollectorType}, {nameof(Highlights)}.Count: {Highlights?.Count}";
}