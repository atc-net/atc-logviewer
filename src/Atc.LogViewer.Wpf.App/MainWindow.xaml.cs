namespace Atc.LogViewer.Wpf.App;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow
{
    public MainWindow(
        MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm!.OnLoaded(this, e);
    }

    private void OnClosing(
        object? sender,
        CancelEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm!.OnClosing(this, e);
    }

    private void OnKeyDown(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm!.OnKeyDown(this, e);
    }

    private void OnKeyUp(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm!.OnKeyUp(this, e);
    }

    private void OnFilterTextKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (sender is not TextBox textBox)
        {
            return;
        }

        var binding = textBox.GetBindingExpression(TextBox.TextProperty);
        binding?.UpdateSource();
    }

    private void OnListViewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.C || (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
        {
            return;
        }

        var listView = sender as ListView;
        if (listView?.SelectedItem is AtcLogEntryEx selectedItem)
        {
            Clipboard.SetText(selectedItem.Message);
        }
    }
}