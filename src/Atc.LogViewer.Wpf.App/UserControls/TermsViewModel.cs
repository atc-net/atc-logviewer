namespace Atc.LogViewer.Wpf.App.UserControls;

public class TermsViewModel()
    : ViewModelBase
{
    private string? selectedTerm;
    private string? newTerm;

    public TermsViewModel(
        IEnumerable<string> terms)
        : this()
    {
        ArgumentNullException.ThrowIfNull(terms);

        Terms.AddRange(terms);
        if (Terms.Count > 0)
        {
            SelectedTerm = Terms[0];
        }
    }

    public ObservableCollectionEx<string> Terms { get; set; } = [];

    public string? NewTerm
    {
        get => newTerm;
        set
        {
            if (value == newTerm)
            {
                return;
            }

            newTerm = value;
            RaisePropertyChanged();
        }
    }

    public string? SelectedTerm
    {
        get => selectedTerm;
        set
        {
            if (value == selectedTerm)
            {
                return;
            }

            selectedTerm = value;
            RaisePropertyChanged();
        }
    }

    public ICommand AddCommand => new RelayCommand(AddCommandHandler, CanAddCommandHandler);

    public ICommand DeleteCommand => new RelayCommand(DeleteCommandHandler, CanDeleteCommandHandler);

    private bool CanAddCommandHandler()
        => !string.IsNullOrEmpty(NewTerm) &&
           !Terms.Contains(NewTerm, StringComparer.OrdinalIgnoreCase);

    private void AddCommandHandler()
    {
        if (!CanAddCommandHandler())
        {
            return;
        }

        Terms.Add(NewTerm!);
        SelectedTerm = NewTerm;
    }

    private bool CanDeleteCommandHandler()
        => SelectedTerm is not null;

    private void DeleteCommandHandler()
    {
        if (!CanDeleteCommandHandler())
        {
            return;
        }

        Terms.Remove(SelectedTerm!);
        SelectedTerm = null;
    }
}