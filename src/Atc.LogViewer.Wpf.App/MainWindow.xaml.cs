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
        Drop += OnDrop;
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

    private void OnDrop(
        object sender,
        DragEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm!.OnDrop(this, e);
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
            Clipboard.SetText(selectedItem.MessageShort);
        }
    }

    private void OnContextMenuOpening(
        object sender,
        ContextMenuEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement { DataContext: AtcLogEntryEx item } sourceElement)
        {
            return;
        }

        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var contextMenu = new ContextMenu();

        contextMenu.Items.Add(
            new MenuItem
            {
                Header = "Filter on item message",
                Command = viewModel.SetMessageToFilterTextCommand,
                CommandParameter = item.MessageShort,
            });

        contextMenu.Items.Add(
            new MenuItem
            {
                Header = "Filter on item timestamp - from",
                Command = viewModel.SetMessageToFilterFromCommand,
                CommandParameter = item.TimeStamp,
            });

        contextMenu.Items.Add(
            new MenuItem
            {
                Header = "Filter on item timestamp - to",
                Command = viewModel.SetMessageToFilterToCommand,
                CommandParameter = item.TimeStamp,
            });

        contextMenu.Items.Add(
            new MenuItem
            {
                Header = "Copy to Clipboard",
                Command = viewModel.CopyMessageToClipboardCommand,
                CommandParameter = item.MessageShort,
                InputGestureText = "Ctrl+C",
            });

        sourceElement.ContextMenu = contextMenu;
    }
}