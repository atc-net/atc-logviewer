namespace Atc.LogViewer.Wpf.App.Dialogs;

/// <summary>
/// Interaction logic for AboutBoxDialog.xaml
/// </summary>
public partial class AboutBoxDialog
{
    public AboutBoxDialog()
    {
        InitializeComponent();

        IconImage.Source = App.DefaultIcon;

        VersionTextBlock.Text = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version!
            .ToString();
    }

    private void OnOk(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}