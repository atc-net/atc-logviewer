// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.LogViewer.Wpf.App;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "OK.")]
public partial class MainWindowViewModel
{
    public IRelayCommandAsync OpenProfileCommand => new RelayCommandAsync(OpenProfileCommandHandler);

    public IRelayCommandAsync OpenLastUsedProfileCommand => new RelayCommandAsync(OpenLastUsedProfileCommandHandler, CanOpenLastUsedProfileCommandHandler);

    public IRelayCommandAsync OpenLogFolderCommand => new RelayCommandAsync(OpenLogFolderCommandHandler);

    public IRelayCommandAsync NewProfileCommand => new RelayCommandAsync(NewProfileCommandHandler);

    public IRelayCommandAsync EditProfileCommand => new RelayCommandAsync(EditProfileCommandHandler, CanEditProfileCommandHandler);

    public IRelayCommandAsync SaveProfileCommand => new RelayCommandAsync(SaveProfileCommandHandler, CanSaveProfileCommandHandler);

    public IRelayCommand OpenApplicationSettingsCommand => new RelayCommand(OpenApplicationSettingsCommandHandler);

    public IRelayCommand OpenApplicationCheckForUpdatesCommand => new RelayCommand(OpenApplicationCheckForUpdatesCommandHandler, CanOpenApplicationCheckForUpdatesCommandHandler);

    public IRelayCommand OpenApplicationAboutCommand => new RelayCommand(OpenApplicationAboutCommandHandler);

    public IRelayCommand<string> SetMessageToFilterTextCommand => new RelayCommand<string>(SetMessageToFilterTextCommandHandler);

    public IRelayCommand<DateTime> SetMessageToFilterFromCommand => new RelayCommand<DateTime>(SetMessageToFilterFromCommandHandler);

    public IRelayCommand<DateTime> SetMessageToFilterToCommand => new RelayCommand<DateTime>(SetMessageToFilterToCommandHandler);

    public IRelayCommand<string> CopyMessageToClipboardCommand => new RelayCommand<string>(CopyMessageToClipboardCommandHandler);

    public IRelayCommand ClearFilterTextCommand => new RelayCommand(ClearFilterTextCommandHandler, CanClearFilterTextCommandHandler);

    public IRelayCommand ClearFilterCommand => new RelayCommand(ClearFilterCommandHandler, CanClearFilterCommandHandler);

    public IRelayCommandAsync EditHighlightsCommand => new RelayCommandAsync(EditHighlightsCommandHandler, CanEditHighlightsCommandHandler);

    public IRelayCommand<ViewMode> ChangeViewModeCommand => new RelayCommand<ViewMode>(ChangeViewModeCommandHandler, CanChangeViewModeCommandHandler);

    private async Task OpenProfileCommandHandler()
    {
        var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = App.LogViewerProgramDataProfilesDirectory.FullName,
            Multiselect = false,
            Title = "Select a profile file",
            Filter = "Profile Files|*.json",
        };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        await LoadProfileFile(
            new FileInfo(openFileDialog.FileName),
            CancellationToken.None).ConfigureAwait(true);
    }

    private bool CanOpenLastUsedProfileCommandHandler()
        => RecentOpenFiles.Any();

    private async Task OpenLastUsedProfileCommandHandler()
    {
        if (!CanOpenLastUsedProfileCommandHandler())
        {
            return;
        }

        await LoadProfileFile(
            new FileInfo(RecentOpenFiles[0].File),
            CancellationToken.None)
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task NewProfileCommandHandler()
    {
        var dialogBox = DialogBoxFactory.CreateNewProfile();
        var dialogResult = dialogBox.ShowDialog();

        if (dialogResult != true)
        {
            return;
        }

        var fileName = DialogBoxExtractor.ExtractProfileFileName(dialogBox);

        var file = new FileInfo(Path.Combine(App.LogViewerProgramDataProfilesDirectory.FullName, fileName));
        if (file.Exists)
        {
            var overrideDialogBox = new QuestionDialogBox(
                Application.Current.MainWindow!,
                "File exist - override it?");

            var overrideDialogResult = overrideDialogBox.ShowDialog();
            if (overrideDialogResult is null ||
                !overrideDialogResult.Value)
            {
                return;
            }
        }

        var (extractProfileViewModel, warningMessage) = DialogBoxExtractor.ExtractProfileViewModel(dialogBox);
        if (extractProfileViewModel is null)
        {
            var warningDialogBox = DialogBoxFactory.CreateWarningOption(warningMessage!);
            warningDialogBox.ShowDialog();
            return;
        }

        ProfileViewModel = extractProfileViewModel;

        profileFile = file;
        await SaveProfileCommandHandler()
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private bool CanEditProfileCommandHandler()
        => ProfileViewModel.CollectorType != LogFileCollectorType.None;

    private async Task EditProfileCommandHandler()
    {
        var dialogBox = DialogBoxFactory.CreateEditProfile(ProfileViewModel);
        var dialogResult = dialogBox.ShowDialog();

        if (dialogResult != true)
        {
            return;
        }

        var (extractProfileViewModel, warningMessage) = DialogBoxExtractor.ExtractProfileViewModel(dialogBox);
        if (extractProfileViewModel is null)
        {
            var warningDialogBox = DialogBoxFactory.CreateWarningOption(warningMessage!);
            warningDialogBox.ShowDialog();
            return;
        }

        ProfileViewModel = extractProfileViewModel;

        await SaveProfileCommandHandler()
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private bool CanSaveProfileCommandHandler()
        => profileFile is not null;

    private async Task SaveProfileCommandHandler()
    {
        if (!CanSaveProfileCommandHandler())
        {
            return;
        }

        await SaveProfileFile(
                profileFile!,
                ProfileViewModel,
                CancellationToken.None)
            .ConfigureAwait(continueOnCapturedContext: false);

        await LoadLogFolder(
                new DirectoryInfo(ProfileViewModel.LogFolder),
                ProfileViewModel.CollectorConfiguration,
                CancellationToken.None)
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private void OpenApplicationSettingsCommandHandler()
    {
        var dialogBox = new BasicApplicationSettingsDialogBox
        {
            DataContext = new BasicApplicationSettingsDialogBoxViewModel(ApplicationOptions),
        };

        var dialogResult = dialogBox.ShowDialog();
        if (!dialogResult.HasValue ||
            !dialogResult.Value ||
            dialogBox.DataContext is not BasicApplicationSettingsDialogBoxViewModel vm)
        {
            return;
        }

        ApplicationOptions = vm.ApplicationSettings.Clone();
        var applicationOptionsAsJson = vm.ToJson();
        var customAppSettingsFile = new FileInfo(Path.Combine(App.LogViewerCommonApplicationDataDirectory.FullName, AtcFileNameConstants.AppSettingsCustom));
        FileHelper.WriteAllText(customAppSettingsFile, applicationOptionsAsJson);
    }

    private static bool CanOpenApplicationCheckForUpdatesCommandHandler()
        => NetworkInformationHelper.HasConnection();

    private void OpenApplicationCheckForUpdatesCommandHandler()
        => new CheckForUpdatesBoxDialog(checkForUpdatesBoxDialogViewModel).ShowDialog();

    private async Task OpenLogFolderCommandHandler()
    {
        var openFolderDialog = new OpenFolderDialog
        {
            InitialDirectory = @"C:\",
            Multiselect = false,
            Title = "Select a log folder",
        };

        if (openFolderDialog.ShowDialog() != true)
        {
            return;
        }

        await LoadLogFolder(
                new DirectoryInfo(openFolderDialog.FolderName),
                ProfileViewModel.CollectorConfiguration,
                CancellationToken.None)
            .ConfigureAwait(true);
    }

    private void OpenApplicationAboutCommandHandler()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var aboutBoxDialog = new AboutBoxDialog();
        aboutBoxDialog.IconImage.Source = App.DefaultIcon;
        aboutBoxDialog.ShowDialog();
    }

    private void SetMessageToFilterTextCommandHandler(
        string obj)
        => FilterText = obj;

    private void SetMessageToFilterFromCommandHandler(
        DateTime obj)
        => FilterDateTimeFrom = obj;

    private void SetMessageToFilterToCommandHandler(
        DateTime obj)
        => FilterDateTimeTo = obj;

    private void CopyMessageToClipboardCommandHandler(
        string obj)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Clipboard.SetText(obj);
        });
    }

    private bool CanClearFilterTextCommandHandler()
        => !string.IsNullOrEmpty(FilterText);

    private void ClearFilterTextCommandHandler()
        => FilterText = string.Empty;

    private bool CanClearFilterCommandHandler()
        => !string.IsNullOrEmpty(FilterText) ||
           FilterDateTimeFrom is not null ||
           FilterDateTimeTo is not null ||
           !string.IsNullOrEmpty(SelectedSourceSystemKey);

    private void ClearFilterCommandHandler()
    {
        filterText = string.Empty;
        filterDateTimeFrom = null;
        filterDateTimeTo = null;
        selectedSourceSystemKey = string.Empty;

        _ = ApplyFilter();

        RaisePropertyChanged(nameof(FilterText));
        RaisePropertyChanged(nameof(FilterDateTimeFrom));
        RaisePropertyChanged(nameof(FilterDateTimeTo));
        RaisePropertyChanged(nameof(SelectedSourceSystemKey));
    }

    private bool CanEditHighlightsCommandHandler()
        => ProfileViewModel is not null;

    private async Task EditHighlightsCommandHandler()
    {
        if (!CanEditHighlightsCommandHandler())
        {
            return;
        }

        var dialogBox = new HighlightEditorDialogBox
        {
            HighlightEditorViewModel = new HighlightEditorViewModel(ProfileViewModel.Highlights),
        };

        dialogBox.ShowDialog();
        if (dialogBox.DialogResult != true)
        {
            return;
        }

        if (dialogBox.DataContext is not HighlightEditorDialogBox highlightEditorDialogBox)
        {
            return;
        }

        ProfileViewModel.Highlights = highlightEditorDialogBox.HighlightEditorViewModel.Highlights;

        await ApplyFilter()
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    private bool CanChangeViewModeCommandHandler(
        ViewMode obj)
        => ViewMode != obj;

    private void ChangeViewModeCommandHandler(
        ViewMode obj)
    {
        ViewMode = obj;
        switch (obj)
        {
            case ViewMode.ChartTimeline:
                ChartTimelineViewModel.RefreshChart();
                break;
        }
    }
}