namespace Atc.LogViewer.Wpf.App.Dialogs;

public class CheckForUpdatesBoxDialogViewModel : ViewModelBase, ICheckForUpdatesBoxDialogViewModel, IDisposable
{
    private readonly IGitHubReleaseService gitHubReleaseService;
    private readonly CancellationTokenSource? cancellationTokenSource;
    private string latestVersion = string.Empty;
    private string latestLink = string.Empty;
    private bool hasNewVersion;

    public CheckForUpdatesBoxDialogViewModel(
        IGitHubReleaseService gitHubReleaseService)
    {
        this.gitHubReleaseService = gitHubReleaseService;

        CurrentVersion = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version!
            .ToString();

        LatestVersion = CurrentVersion;

        cancellationTokenSource = new CancellationTokenSource();
        Task.Run(
            async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (NetworkInformationHelper.HasConnection())
                    {
                        TaskHelper.RunSync(RetrieveLatestFromGitHubHandler);
                    }

                    await Task
                        .Delay(TimeSpan.FromHours(1), CancellationToken.None)
                        .ConfigureAwait(true);
                }
            },
            cancellationTokenSource.Token);
    }

    public IRelayCommandAsync DownloadLatestCommand
        => new RelayCommandAsync(
            DownloadLatestCommandHandler,
            CanDownloadLatestCommandHandler);

    public static IRelayCommand<NiceDialogBox> CancelCommand
        => new RelayCommand<NiceDialogBox>(
            CancelCommandHandler);

    public string CurrentVersion { get; set; }

    public string LatestVersion
    {
        get => latestVersion;
        set
        {
            latestVersion = value;
            RaisePropertyChanged();
        }
    }

    public string LatestLink
    {
        get => latestLink;
        set
        {
            latestLink = value;
            RaisePropertyChanged();

            HasNewVersion = false;
            if (Version.TryParse(CurrentVersion, out var cv) &&
                Version.TryParse(LatestVersion, out var lv))
            {
                HasNewVersion = lv.GreaterThan(cv);
            }
        }
    }

    public bool HasNewVersion
    {
        get => hasNewVersion;
        set
        {
            hasNewVersion = value;
            RaisePropertyChanged();
        }
    }

    private async Task RetrieveLatestFromGitHubHandler()
    {
        var version = await gitHubReleaseService
            .GetLatestVersion()
            .ConfigureAwait(true);

        if (version is not null)
        {
            LatestVersion = version.ToString();
            var link = await gitHubReleaseService
                .GetLatestMsiLink()
                .ConfigureAwait(true);

            if (link is not null)
            {
                LatestLink = link.AbsoluteUri;
            }
        }
    }

    private bool CanDownloadLatestCommandHandler()
        => HasNewVersion;

    private async Task DownloadLatestCommandHandler()
    {
        var downloadBytes = await gitHubReleaseService
            .DownloadFileByLink(new Uri(LatestLink))
            .ConfigureAwait(true);

        if (downloadBytes.Length > 0)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "Atc.LogViewer.msi",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                await File
                    .WriteAllBytesAsync(saveFileDialog.FileName, downloadBytes, CancellationToken.None)
                    .ConfigureAwait(true);
            }
        }
    }

    private static void CancelCommandHandler(
        NiceDialogBox dialogBox)
    {
        dialogBox.Close();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        cancellationTokenSource?.Dispose();
    }
}