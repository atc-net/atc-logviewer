namespace Atc.LogViewer.Wpf.App.Models;

public class ProfileViewModel : ViewModelBase
{
    public string Name { get; set; } = string.Empty;

    public string LogFolder { get; set; } = string.Empty;

    public LogFileCollectorType CollectorType { get; set; }

    public LogFileCollectorConfiguration CollectorConfiguration { get; set; } = new();

    public ObservableCollectionEx<HighlightViewModel> Highlights { get; set; } = new();

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(LogFolder)}: {LogFolder}, {nameof(CollectorType)}: {CollectorType}, {nameof(CollectorConfiguration)}: ({CollectorConfiguration}), {nameof(Highlights)}.Count: {Highlights.Count}";
}