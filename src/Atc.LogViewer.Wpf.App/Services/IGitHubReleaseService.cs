namespace Atc.LogViewer.Wpf.App.Services;

public interface IGitHubReleaseService
{
    Task<Version?> GetLatestVersion();

    Task<Uri?> GetLatestMsiLink();

    Task<byte[]> DownloadFileByLink(Uri uri);
}