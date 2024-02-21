namespace Atc.LogViewer.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase
{
    private readonly ILogger<MainWindowViewModel> logger;
    private readonly ILogAnalyzer logAnalyzer;
    private BitmapImage? icon;
    private FileInfo? profileFile;
    private bool followTail = true;
    private bool isTraceEnabled = true;
    private bool isDebugEnabled = true;
    private bool isInfoEnabled = true;
    private bool isWarningEnabled = true;
    private bool isErrorEnabled = true;
    private bool isCriticalEnabled = true;
    private string filterText = string.Empty;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        ILogAnalyzer logAnalyzer,
        StatusBarViewModel statusBarViewModel)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(logAnalyzer);
        ArgumentNullException.ThrowIfNull(statusBarViewModel);

        this.logger = logger;
        this.logAnalyzer = logAnalyzer;

        Icon = App.DefaultIcon;

        ProfileViewModel = new ProfileViewModel();
        StatusBarViewModel = statusBarViewModel;

        LoadRecentOpenFiles();

        this.logAnalyzer.SetFilter(
            new LogFilter(
                IsTraceEnabled,
                IsDebugEnabled,
                IsInfoEnabled,
                IsWarningEnabled,
                IsErrorEnabled,
                IsCriticalEnabled,
                FilterText));

        this.logAnalyzer.CollectedEntry += OnCollectedEntry;
        this.logAnalyzer.CollectedEntries += OnCollectedEntries;
    }

    public BitmapImage? Icon
    {
        get => icon;
        set
        {
            icon = value;
            RaisePropertyChanged();
        }
    }

    public ProfileViewModel ProfileViewModel { get; set; }

    public StatusBarViewModel StatusBarViewModel { get; set; }

    public ObservableCollectionEx<RecentOpenFileViewModel> RecentOpenFiles { get; } = new();

    public ObservableCollectionEx<AtcLogEntryEx> LogEntries { get; } = new();

    public bool FollowTail
    {
        get => followTail;
        set
        {
            followTail = value;
            RaisePropertyChanged();
        }
    }

    public bool IsTraceEnabled
    {
        get => isTraceEnabled;
        set
        {
            isTraceEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public bool IsDebugEnabled
    {
        get => isDebugEnabled;
        set
        {
            isDebugEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public bool IsInfoEnabled
    {
        get => isInfoEnabled;
        set
        {
            isInfoEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public bool IsWarningEnabled
    {
        get => isWarningEnabled;
        set
        {
            isWarningEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public bool IsErrorEnabled
    {
        get => isErrorEnabled;
        set
        {
            isErrorEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public bool IsCriticalEnabled
    {
        get => isCriticalEnabled;
        set
        {
            isCriticalEnabled = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public string FilterText
    {
        get => filterText;
        set
        {
            filterText = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public new void OnKeyDown(
        object sender,
        KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(sender, e);

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.N)
        {
            _ = NewProfileCommandHandler().ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.P)
        {
            _ = OpenProfileCommandHandler().ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.R)
        {
            _ = OpenLastUsedProfileCommandHandler().ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.O)
        {
            _ = OpenLogFolderCommandHandler().ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.S &&
            CanSaveProfileCommandHandler())
        {
            _ = SaveProfileCommandHandler().ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private void OnCollectedEntry(
        AtcLogEntry logEntry)
    {
        var data = GetLogEntryEx(logEntry);

        Application.Current.Dispatcher.Invoke(() =>
        {
            LogEntries.Add(data);
        });

        CountLogEntriesStatsAndSend();
    }

    private void OnCollectedEntries(
        AtcLogEntry[] logEntries)
    {
        var data = logEntries
            .Select(GetLogEntryEx)
            .ToList();

        Application.Current.Dispatcher.Invoke(() =>
        {
            LogEntries.SuppressOnChangedNotification = true;

            LogEntries.AddRange(data);

            LogEntries.SuppressOnChangedNotification = false;
        });

        CountLogEntriesStatsAndSend();
    }

    private async Task ApplyFilter()
    {
        await SetIsBusy(value: true, delayInMs: 10).ConfigureAwait(false);

        logAnalyzer.SetFilter(
            new LogFilter(
                IsTraceEnabled,
                IsDebugEnabled,
                IsInfoEnabled,
                IsWarningEnabled,
                IsErrorEnabled,
                IsCriticalEnabled,
                FilterText));

        var filteredEntries = logAnalyzer.GetFilteredLogEntries();

        LogEntries.SuppressOnChangedNotification = true;

        LogEntries.Clear();
        foreach (var logEntry in filteredEntries)
        {
            var logEntryEx = GetLogEntryEx(logEntry);
            LogEntries.Add(logEntryEx);
        }

        LogEntries.SuppressOnChangedNotification = false;

        CountLogEntriesStatsAndSend();

        IsBusy = false;
    }

    private AtcLogEntryEx GetLogEntryEx(
        AtcLogEntry logEntry)
    {
        var foreground = Brushes.White;
        var background = Brushes.Transparent;

        foreach (var highlight in ProfileViewModel.Highlights)
        {
            var stringComparison = StringComparison.Ordinal;
            if (highlight.IgnoreCasing)
            {
                stringComparison = StringComparison.OrdinalIgnoreCase;
            }

            if (logEntry.Message.Contains(highlight.Text, stringComparison))
            {
                foreground = highlight.Foreground;
                background = highlight.Background;
                break;
            }
        }

        var logEntryEx = new AtcLogEntryEx(
            logEntry.SourceIdentifier,
            logEntry.DateTime,
            logEntry.LogLevel,
            logEntry.Message,
            foreground,
            background);
        return logEntryEx;
    }
}