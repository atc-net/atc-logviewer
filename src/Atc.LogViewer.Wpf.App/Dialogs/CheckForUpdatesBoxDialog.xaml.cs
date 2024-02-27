namespace Atc.LogViewer.Wpf.App.Dialogs;

/// <summary>
/// Interaction logic for CheckForUpdatesBoxDialog.
/// </summary>
public partial class CheckForUpdatesBoxDialog
{
    public CheckForUpdatesBoxDialog(
        ICheckForUpdatesBoxDialogViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}