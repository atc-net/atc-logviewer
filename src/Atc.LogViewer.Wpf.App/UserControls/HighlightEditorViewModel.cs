namespace Atc.LogViewer.Wpf.App.UserControls;

public class HighlightEditorViewModel()
    : ViewModelBase
{
    private HighlightViewModel? selectedHighlight;

    public HighlightEditorViewModel(
        IEnumerable<HighlightViewModel> highlights)
        : this()
    {
        ArgumentNullException.ThrowIfNull(highlights);

        Highlights.AddRange(highlights);
        if (Highlights.Count > 0)
        {
            SelectedHighlight = Highlights[0];
        }
    }

    public ObservableCollectionEx<HighlightViewModel> Highlights { get; set; } = new();

    public HighlightViewModel? SelectedHighlight
    {
        get => selectedHighlight;
        set
        {
            selectedHighlight = value;
            RaisePropertyChanged();
        }
    }

    public ICommand AddCommand => new RelayCommand(AddCommandHandler);

    public ICommand DeleteCommand => new RelayCommand(DeleteCommandHandler, CanDeleteCommandHandler);

    private void AddCommandHandler()
    {
        var newHighlight = new HighlightViewModel();
        Highlights.Add(newHighlight);
        SelectedHighlight = newHighlight;
    }

    private bool CanDeleteCommandHandler()
        => SelectedHighlight is not null;

    private void DeleteCommandHandler()
    {
        if (!CanDeleteCommandHandler())
        {
            return;
        }

        Highlights.Remove(SelectedHighlight!);
        SelectedHighlight = null;
    }
}