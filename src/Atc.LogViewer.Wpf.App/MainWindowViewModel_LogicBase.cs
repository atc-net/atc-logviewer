// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.LogViewer.Wpf.App;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "OK.")]
public partial class MainWindowViewModel
{
    private void LoadRecentOpenFiles()
    {
        var recentOpenFilesFile = Path.Combine(App.LogViewerProgramDataRecentProfilesDirectory.FullName, Constants.RecentOpenFilesFileName);
        if (!File.Exists(recentOpenFilesFile))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(recentOpenFilesFile);

            var recentOpenFilesOption = JsonSerializer.Deserialize<RecentOpenFilesOption>(
                json,
                App.JsonSerializerOptions) ?? throw new IOException($"Invalid format in {recentOpenFilesFile}");

            RecentOpenFiles.Clear();

            RecentOpenFiles.SuppressOnChangedNotification = true;
            foreach (var recentOpenFile in recentOpenFilesOption.RecentOpenFiles.OrderByDescending(x => x.TimeStamp))
            {
                if (!File.Exists(recentOpenFile.FilePath))
                {
                    continue;
                }

                RecentOpenFiles.Add(new RecentOpenFileViewModel(App.LogViewerProgramDataRecentProfilesDirectory, recentOpenFile.TimeStamp, recentOpenFile.FilePath));
            }

            RecentOpenFiles.SuppressOnChangedNotification = false;
        }
        catch
        {
            // Skip
        }
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

        var recentOpenFilesFilePath = Path.Combine(App.LogViewerProgramDataRecentProfilesDirectory.FullName, Constants.RecentOpenFilesFileName);
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
        CancellationToken cancellationToken)
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

            var defaultDirectory = new DirectoryInfo(ProfileViewModel.DefaultLogFolder);
            if (defaultDirectory.Exists)
            {
                var logFileCollectorConfig = new LogFileCollectorConfig();

                await LoadLogFolder(
                    defaultDirectory,
                    logFileCollectorConfig,
                    cancellationToken)
                    .ConfigureAwait(false);
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
        CancellationToken cancellationToken)
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
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, $"Profile file: {file.FullName}, Error: {ex.Message}");
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
        }
    }

    private async Task LoadLogFolder(
        DirectoryInfo directory,
        LogFileCollectorConfig config,
        CancellationToken cancellationToken)
    {
        // TODO: Analyze files or/and show Dialog
        var useLogCollectorType = LogFileCollectorType.Serilog;

        IsBusy = true;

        logAnalyzer.ClearLogEntries();
        LogEntries.Clear();

        await logAnalyzer
            .CollectAndMonitorFolder(
                useLogCollectorType,
                directory,
                config,
                cancellationToken)
            .ConfigureAwait(false);

        IsBusy = false;
    }

    private void CountLogEntriesStatsAndSend()
    {
        var logStatistics = logAnalyzer.GetLogStatistics();

        Messenger.Default.Send(
            new LogEntriesStatsMessage(
                Count: logStatistics.Count,
                TotalCount: LogEntries.Count,
                CriticalCount: LogEntries.Count(x => x.LogLevel == LogLevel.Critical),
                TotalCriticalCount: logStatistics.CriticalCount,
                ErrorCount: LogEntries.Count(x => x.LogLevel == LogLevel.Error),
                TotalErrorCount: logStatistics.ErrorCount,
                WarningCount: LogEntries.Count(x => x.LogLevel == LogLevel.Warning),
                TotalWarningCount: logStatistics.WarningCount,
                InformationCount: LogEntries.Count(x => x.LogLevel == LogLevel.Information),
                TotalInformationCount: logStatistics.InformationCount,
                DebugCount: LogEntries.Count(x => x.LogLevel == LogLevel.Debug),
                TotalDebugCount: logStatistics.DebugCount,
                TraceCount: LogEntries.Count(x => x.LogLevel == LogLevel.Trace),
                TotalTraceCount: logStatistics.TraceCount));
    }
}