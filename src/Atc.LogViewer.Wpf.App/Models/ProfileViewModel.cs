namespace Atc.LogViewer.Wpf.App.Models;

public class ProfileViewModel : ViewModelBase
{
    public string DefaultLogFolder { get; set; } = string.Empty;

    public LogFileCollectorType DefaultCollectorType { get; set; }

    public ObservableCollectionEx<HighlightViewModel> Highlights { get; set; } = new();
}