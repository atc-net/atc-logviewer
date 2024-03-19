using Atc.Wpf.Controls.SettingsControls;
using Atc.Wpf.Theming;
using Atc.Wpf.Theming.Helpers;

namespace Atc.LogViewer.Wpf.App;

public partial class MainWindowViewModel : MainWindowViewModelBase
{
    private readonly ILogger<MainWindowViewModel> logger;
    private readonly ILogAnalyzer logAnalyzer;
    private readonly IGitHubReleaseService gitHubReleaseService;
    private readonly ICheckForUpdatesBoxDialogViewModel checkForUpdatesBoxDialogViewModel;
    private string? newVersionIsAvailable;
    private BitmapImage? icon;
    private FileInfo? profileFile;
    private bool followTail = true;
    private AtcLogEntryEx? selectedLogEntry;
    private ViewMode viewMode;
    private bool isTraceEnabled = true;
    private bool isDebugEnabled = true;
    private bool isInfoEnabled = true;
    private bool isWarningEnabled = true;
    private bool isErrorEnabled = true;
    private bool isCriticalEnabled = true;
    private string filterText = string.Empty;
    private DateTime? filterDateTimeFrom;
    private DateTime? filterDateTimeTo;
    private string selectedSourceSystemKey = string.Empty;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        ILogAnalyzer logAnalyzer,
        IGitHubReleaseService gitHubReleaseService,
        ICheckForUpdatesBoxDialogViewModel checkForUpdatesBoxDialogViewModel,
        StatusBarViewModel statusBarViewModel,
        IOptions<BasicApplicationOptions> applicationOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(logAnalyzer);
        ArgumentNullException.ThrowIfNull(gitHubReleaseService);
        ArgumentNullException.ThrowIfNull(checkForUpdatesBoxDialogViewModel);
        ArgumentNullException.ThrowIfNull(statusBarViewModel);
        ArgumentNullException.ThrowIfNull(applicationOptions);

        this.logger = logger;
        this.logAnalyzer = logAnalyzer;
        this.gitHubReleaseService = gitHubReleaseService;
        this.checkForUpdatesBoxDialogViewModel = checkForUpdatesBoxDialogViewModel;

        Icon = App.DefaultIcon;

        ApplicationOptions = new BasicApplicationSettingsViewModel(applicationOptions.Value)
        {
            ShowLanguage = false,
        };

        ProfileViewModel = new ProfileViewModel();
        ChartLogLevelPieViewModel = new ChartLogLevelPieViewModel();
        ChartTimelineViewModel = new ChartTimelineViewModel(LogEntries);
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
                FilterText,
                FilterDateTimeFrom,
                FilterDateTimeTo,
                SelectedSourceSystemKey));

        this.logAnalyzer.CollectedEntry += OnCollectedEntry;
        this.logAnalyzer.CollectedEntries += OnCollectedEntries;

        if (ApplicationOptions.OpenRecentFileOnStartup &&
            RecentOpenFiles.Count > 0)
        {
            OpenLastUsedProfileCommand.ExecuteAsync(parameter: null);
        }

        Task.Factory.StartNew(
            async () => await CheckForUpdates().ConfigureAwait(false),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    public string? NewVersionIsAvailable
    {
        get => newVersionIsAvailable;
        set
        {
            newVersionIsAvailable = value;
            OnPropertyChanged();
        }
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

    public BasicApplicationSettingsViewModel ApplicationOptions { get; set; }

    public ProfileViewModel ProfileViewModel { get; set; }

    public ChartLogLevelPieViewModel ChartLogLevelPieViewModel { get; set; }

    public ChartTimelineViewModel ChartTimelineViewModel { get; set; }

    public StatusBarViewModel StatusBarViewModel { get; set; }

    public ObservableCollectionEx<RecentOpenFileViewModel> RecentOpenFiles { get; } = [];

    public ObservableCollectionEx<AtcLogEntryEx> LogEntries { get; } = [];

    public IDictionary<string, string> SourceSystems { get; private set; } = new Dictionary<string, string>(StringComparer.Ordinal);

    public string SelectedSourceSystemKey
    {
        get => selectedSourceSystemKey;
        set
        {
            selectedSourceSystemKey = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public AtcLogEntryEx? SelectedLogEntry
    {
        get => selectedLogEntry;
        set
        {
            selectedLogEntry = value;
            RaisePropertyChanged();
        }
    }

    public static CultureInfo DateTimePickerUiCulture => GlobalizationConstants.DanishCultureInfo;

    public ViewMode ViewMode
    {
        get => viewMode;
        set
        {
            viewMode = value;
            RaisePropertyChanged();
        }
    }

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

    public DateTime? FilterDateTimeFrom
    {
        get => filterDateTimeFrom;
        set
        {
            filterDateTimeFrom = value;
            RaisePropertyChanged();
            _ = ApplyFilter();
        }
    }

    public DateTime? FilterDateTimeTo
    {
        get => filterDateTimeTo;
        set
        {
            filterDateTimeTo = value;
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
            _ = NewProfileCommandHandler()
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.P)
        {
            _ = OpenProfileCommandHandler()
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.R)
        {
            _ = OpenLastUsedProfileCommandHandler()
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.O)
        {
            _ = OpenLogFolderCommandHandler()
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        if (!e.Handled &&
            Keyboard.Modifiers == ModifierKeys.Control &&
            e.Key == Key.S &&
            CanSaveProfileCommandHandler())
        {
            _ = SaveProfileCommandHandler()
                .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private void OnCollectedEntry(
        AtcLogEntry logEntry)
    {
        if (!SourceSystems.ContainsKey(logEntry.SourceSystem))
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SourceSystems.Clear();
                SourceSystems = logAnalyzer.GetSourceSystems();
                RaisePropertyChanged(nameof(SourceSystems));
            });
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            LogEntries.Add(GetLogEntryEx(logEntry));
        });

        CountLogEntriesStatsAndSend();
    }

    private void OnCollectedEntries(
        AtcLogEntry[] logEntries)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var data = logEntries
                .Select(GetLogEntryEx)
                .ToList();

            LogEntries.SuppressOnChangedNotification = true;

            LogEntries.AddRange(data);

            LogEntries.SuppressOnChangedNotification = false;

            SourceSystems.Clear();
            SourceSystems = logAnalyzer.GetSourceSystems();
            RaisePropertyChanged(nameof(SourceSystems));
        });

        CountLogEntriesStatsAndSend();
    }

    private async Task ApplyFilter()
    {
        await SetIsBusy(value: true, delayInMs: 10)
            .ConfigureAwait(continueOnCapturedContext: false);

        logAnalyzer.SetFilter(
            new LogFilter(
                IsTraceEnabled,
                IsDebugEnabled,
                IsInfoEnabled,
                IsWarningEnabled,
                IsErrorEnabled,
                IsCriticalEnabled,
                FilterText,
                FilterDateTimeFrom,
                FilterDateTimeTo,
                SelectedSourceSystemKey));

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
        var foreground = ThemeManagerHelper.GetBrushByResourceKey(AtcAppsBrushKeyType.ThemeForeground);
        var background = Brushes.Transparent;

        foreach (var highlight in ProfileViewModel.Highlights)
        {
            var stringComparison = StringComparison.Ordinal;
            if (highlight.IgnoreCasing)
            {
                stringComparison = StringComparison.OrdinalIgnoreCase;
            }

            if (!logEntry.MessageFull.Contains(highlight.Text, stringComparison))
            {
                continue;
            }

            foreground = highlight.Foreground;
            background = highlight.Background;
            break;
        }

        return new AtcLogEntryEx(
            logEntry.SourceIdentifier,
            logEntry.SourceSystem,
            logEntry.LineNumber,
            logEntry.TimeStamp,
            logEntry.LogLevel,
            logEntry.MessageShort,
            logEntry.MessageFull,
            foreground,
            background);
    }
}