// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.LogViewer.Wpf.App;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "OK.")]
public partial class MainWindowViewModel
{
    private async Task CheckForUpdates()
    {
        if (!NetworkInformationHelper.HasConnection())
        {
            return;
        }

        var currentVersion = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version!
            .ToString();

        var latestVersion = await gitHubReleaseService
            .GetLatestVersion()
            .ConfigureAwait(false);

        if (latestVersion is null)
        {
            return;
        }

        if (Version.TryParse(currentVersion, out var cv) &&
            Version.TryParse(latestVersion.ToString(), out var lv) &&
            lv.GreaterThan(cv))
        {
            NewVersionIsAvailable = "New version of LogViewer is available";
        }
    }

    private void LoadRecentOpenFiles()
    {
        var recentOpenFileViewModels = RecentOpenFileHelper.Load(App.LogViewerProgramDataRecentProfilesDirectory);

        RecentOpenFiles.Clear();
        RecentOpenFiles.SuppressOnChangedNotification = true;
        RecentOpenFiles.AddRange(recentOpenFileViewModels);
        RecentOpenFiles.SuppressOnChangedNotification = false;
    }

    private void AddLoadedProfileFileToRecentOpenFiles(
        FileInfo file)
    {
        RecentOpenFiles.Add(new RecentOpenFileViewModel(App.LogViewerProgramDataRecentProfilesDirectory, DateTime.Now, file.FullName));

        var recentOpenFilesOption = new RecentOpenFilesOption();
        foreach (var vm in RecentOpenFiles.OrderByDescending(x => x.TimeStamp))
        {
            var item = new RecentOpenFileOption
            {
                TimeStamp = vm.TimeStamp,
                FilePath = vm.File,
            };

            if (recentOpenFilesOption.RecentOpenFiles.FirstOrDefault(x => x.FilePath == item.FilePath) is not null)
            {
                continue;
            }

            if (!File.Exists(item.FilePath))
            {
                continue;
            }

            recentOpenFilesOption.RecentOpenFiles.Add(item);
        }

        var recentOpenFilesFilePath = Path.Combine(App.LogViewerProgramDataRecentProfilesDirectory.FullName, AtcFileNameConstants.RecentOpenFilesFileName);
        if (!Directory.Exists(App.LogViewerProgramDataProfilesDirectory.FullName))
        {
            Directory.CreateDirectory(App.LogViewerProgramDataRecentProfilesDirectory.FullName);
        }

        var json = JsonSerializer.Serialize(recentOpenFilesOption, App.JsonSerializerOptions);
        File.WriteAllText(recentOpenFilesFilePath, json);

        LoadRecentOpenFiles();
    }

    private async Task LoadProfileFile(
        FileInfo file,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.Log(LogLevel.Trace, $"Loading profile file: {file.FullName}");

            var json = await File
                .ReadAllTextAsync(file.FullName, cancellationToken)
                .ConfigureAwait(true);

            var profileOptions = JsonSerializer.Deserialize<ProfileOptions>(
                json,
                App.JsonSerializerOptions) ?? throw new IOException($"Invalid format in {file}");

            profileFile = file;
            ProfileViewModel = profileOptions.ToViewModel();

            AddLoadedProfileFileToRecentOpenFiles(file);

            var logDirectory = new DirectoryInfo(ProfileViewModel.LogFolder);
            if (logDirectory.Exists)
            {
                await ClearDataAndCollectFromFolder(
                        logDirectory,
                        cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, $"Profile file: {file.FullName}, Error: {ex.Message}");
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
        }
    }

    private async Task SaveProfileFile(
        FileInfo file,
        ProfileViewModel profile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.Log(LogLevel.Trace, $"Saving profile file: {file.FullName}");

            var profileOptions = profile.ToOptions();

            var profileJson = JsonSerializer.Serialize(
                profileOptions,
                App.JsonSerializerOptions);

            await FileHelper
                .WriteAllTextAsync(file, profileJson, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, $"Profile file: {file.FullName}, Error: {ex.Message}");
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
        }
    }

    private async Task CollectFromFile(
        FileInfo file)
    {
        IsBusy = true;

        var logFileCollectorType = logAnalyzer.DetermineCollectorType(file);
        if (logFileCollectorType == LogFileCollectorType.None)
        {
            IsBusy = false;

            var dialogBox = new InfoDialogBox(
                Application.Current.MainWindow!,
                new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Warning)
                {
                    TitleBarText = "Invalid log file",
                },
                "This is not a valid log file");

            dialogBox.ShowDialog();
            return;
        }

        ProfileViewModel.CollectorType = logFileCollectorType;

        await logAnalyzer
            .CollectFile(
                ProfileViewModel.CollectorType,
                file,
                ProfileViewModel.CollectorConfiguration,
                ProfileViewModel.CollectorConfiguration.MonitorFiles,
                CancellationToken.None)
            .ConfigureAwait(continueOnCapturedContext: false);

        IsBusy = false;
    }

    private async Task CollectFromFolder(
        DirectoryInfo directory)
    {
        var files = directory.GetFiles();
        foreach (var file in files)
        {
            await CollectFromFile(file).ConfigureAwait(false);
        }
    }

    private async Task ClearDataAndCollectFromFolder(
        DirectoryInfo directory,
        CancellationToken cancellationToken = default)
    {
        IsBusy = true;

        var logFileCollectorType = logAnalyzer.DetermineCollectorType(directory);
        if (logFileCollectorType == LogFileCollectorType.None)
        {
            IsBusy = false;

            var dialogBox = new InfoDialogBox(
                Application.Current.MainWindow!,
                new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Warning)
                {
                    TitleBarText = "Invalid log folder",
                },
                "There is no valid log files in this folder");

            dialogBox.ShowDialog();
            return;
        }

        logAnalyzer.StopMonitoringAndClearLogEntries();
        LogEntries.Clear();

        await logAnalyzer
            .CollectFolder(
                ProfileViewModel.CollectorType,
                directory,
                ProfileViewModel.CollectorConfiguration,
                ProfileViewModel.CollectorConfiguration.MonitorFiles,
                cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        IsBusy = false;
    }

    private void CountLogEntriesStatsAndSend()
    {
        var logStatistics = logAnalyzer.GetLogStatistics();

        long criticalCount = 0;
        long errorCount = 0;
        long warningCount = 0;
        long informationCount = 0;
        long debugCount = 0;
        long traceCount = 0;

        var logLevels = LogEntries.Select(x => x.LogLevel).ToList();
        foreach (var logLevel in logLevels)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    criticalCount++;
                    break;
                case LogLevel.Error:
                    errorCount++;
                    break;
                case LogLevel.Warning:
                    warningCount++;
                    break;
                case LogLevel.Information:
                    informationCount++;
                    break;
                case LogLevel.Debug:
                    debugCount++;
                    break;
                case LogLevel.Trace:
                    traceCount++;
                    break;
                default:
                    throw new SwitchCaseDefaultException(logLevel);
            }
        }

        Messenger.Default.Send(
            new LogEntriesStatsMessage(
                Count: logStatistics.Count,
                TotalCount: LogEntries.Count,
                CriticalCount: criticalCount,
                TotalCriticalCount: logStatistics.CriticalCount,
                ErrorCount: errorCount,
                TotalErrorCount: logStatistics.ErrorCount,
                WarningCount: warningCount,
                TotalWarningCount: logStatistics.WarningCount,
                InformationCount: informationCount,
                TotalInformationCount: logStatistics.InformationCount,
                DebugCount: debugCount,
                TotalDebugCount: logStatistics.DebugCount,
                TraceCount: traceCount,
                TotalTraceCount: logStatistics.TraceCount));
    }
}