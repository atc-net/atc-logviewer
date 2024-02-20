namespace Atc.LogViewer.Wpf.App;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "OK.")]
public partial class MainWindowViewModel
{
    public IRelayCommandAsync NewProfileCommand => new RelayCommandAsync(NewProfileCommandHandler);

    public IRelayCommandAsync OpenProfileCommand => new RelayCommandAsync(OpenProfileCommandHandler);

    public IRelayCommandAsync SaveProfileCommand => new RelayCommandAsync(SaveProfileCommandHandler, CanSaveProfileCommandHandler);

    public IRelayCommandAsync OpenLogFolderCommand => new RelayCommandAsync(OpenLogFolderCommandHandler);

    public IRelayCommandAsync OpenApplicationSettingsCommand => new RelayCommandAsync(OpenApplicationSettingsCommandHandler);

    public IRelayCommand EditHighlightsCommand => new RelayCommand(EditHighlightsCommandHandler);
    private async Task NewProfileCommandHandler()
    {
        var dialogBox = DialogBoxFactory.CreateNewProfile();
        var dialogResult = dialogBox.ShowDialog();

        if (dialogResult != true)
        {
            var data = dialogBox.Data.GetKeyValues();

            var name = data["Name"].ToString()!;
            if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                name += ".json";
            }

            var defaultLogFolder = data["DefaultLogFolder"].ToString()!;

            if (!Enum<LogFileCollectorType>.TryParse(
                    data["DefaultCollector"].ToString()!,
                    ignoreCase: false,
                    out var defaultCollectorType))
            {
                var warningDialogBox = DialogBoxFactory.CreateWarningOption("Default Collector - was not selected");
                warningDialogBox.ShowDialog();
                return;
            }

            var file = new FileInfo(Path.Combine(App.LogViewerProgramDataProfilesDirectory.FullName, name));
            if (file.Exists)
            {
                var overrideDialogBox = new QuestionDialogBox(
                    Application.Current.MainWindow!,
                    "File exist - override it?");

                var overrideDialogResult = overrideDialogBox.ShowDialog();
                if (overrideDialogResult != true)
                {
                    ProfileViewModel = new ProfileViewModel
                    {
                        DefaultLogFolder = defaultLogFolder,
                        DefaultCollectorType = defaultCollectorType,
                    };

                    profileFile = file;
                    await SaveProfileCommandHandler().ConfigureAwait(false);
                }
            }
            else
            {
                ProfileViewModel = new ProfileViewModel
                {
                    DefaultLogFolder = defaultLogFolder,
                    DefaultCollectorType = defaultCollectorType,
                };

                profileFile = file;
                await SaveProfileCommandHandler().ConfigureAwait(false);
            }
        }
    }

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

    private bool CanSaveProfileCommandHandler()
        => profileFile is not null;

    private Task SaveProfileCommandHandler()
    {
        if (!CanSaveProfileCommandHandler())
        {
            return Task.CompletedTask;
        }

        return SaveProfileFile(
            profileFile!,
            ProfileViewModel,
            CancellationToken.None);
    }

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

        var logFileCollectorConfig = new LogFileCollectorConfig();

        await LoadLogFolder(
            new DirectoryInfo(openFolderDialog.FolderName),
            logFileCollectorConfig,
            CancellationToken.None)
            .ConfigureAwait(true);
    }

    private Task OpenApplicationSettingsCommandHandler()
    {
        throw new NotImplementedException();
    }

    private void EditHighlightsCommandHandler()
    {
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
    }
}