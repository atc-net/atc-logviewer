// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.LogViewer.Wpf.App.Dialogs;

/// <summary>
/// Interaction logic for HighlightEditorDialogBox.
/// </summary>
public partial class HighlightEditorDialogBox
{
    public HighlightEditorDialogBox()
    {
        InitializeComponent();

        DataContext = this;
    }

    public HighlightEditorViewModel HighlightEditorViewModel { get; set; } = new();

    private void OnOk(
        object sender,
        RoutedEventArgs e)
    {
        if (HighlightEditorViewModel.Highlights is null)
        {
            return;
        }

        foreach (var highlight in HighlightEditorViewModel.Highlights)
        {
            if (!string.IsNullOrEmpty(highlight.Text))
            {
                continue;
            }

            var dialogBox = DialogBoxFactory.CreateWarningHighlight();
            dialogBox.ShowDialog();
            return;
        }

        DialogResult = true;
        Close();
    }

    private void OnCancel(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}