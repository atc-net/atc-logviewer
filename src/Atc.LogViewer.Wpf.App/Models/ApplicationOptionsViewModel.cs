namespace Atc.LogViewer.Wpf.App.Models;

public class ApplicationOptionsViewModel : ViewModelBase
{
    private string theme = string.Empty;
    private bool openRecentProfileFileOnStartup;

    public ApplicationOptionsViewModel()
    {
    }

    public ApplicationOptionsViewModel(
        ApplicationOptions applicationOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationOptions);

        theme = applicationOptions.Theme;
        openRecentProfileFileOnStartup = applicationOptions.OpenRecentProfileFileOnStartup;
    }

    public string Theme
    {
        get => theme;
        set
        {
            theme = value;
            IsDirty = true;
            RaisePropertyChanged();
        }
    }

    public bool OpenRecentProfileFileOnStartup
    {
        get => openRecentProfileFileOnStartup;
        set
        {
            if (value == openRecentProfileFileOnStartup)
            {
                return;
            }

            openRecentProfileFileOnStartup = value;
            IsDirty = true;
            RaisePropertyChanged();
        }
    }

    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(IsDirty)}: {IsDirty}";
}