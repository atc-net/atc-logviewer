// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.LogViewer.Wpf.App.Services;

public class GitHubReleaseService : IGitHubReleaseService
{
    private const string UserAgent = "Atc-LogViewer";
    private static readonly Uri ApiUri = new("https://api.github.com/repos/atc-net/atc-logviewer/releases/latest");

    public async Task<Version?> GetLatestVersion()
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            var json = await client
                .GetStringAsync(ApiUri)
                .ConfigureAwait(false);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var versionString = root
                .GetProperty("tag_name")
                .ToString();

            if (versionString.StartsWith('v'))
            {
                versionString = versionString[1..];
            }

            return new Version(versionString);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Uri?> GetLatestMsiLink()
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            var json = await client
                .GetStringAsync(ApiUri)
                .ConfigureAwait(false);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var assets = root
                .GetProperty("assets")
                .EnumerateArray();

            foreach (var asset in assets)
            {
                if (asset
                    .GetProperty("name")
                    .ToString()
                    .EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    return new Uri(asset.GetProperty("browser_download_url").ToString());
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<byte[]> DownloadFileByLink(Uri uri)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            return await client
                .GetByteArrayAsync(uri)
                .ConfigureAwait(false);
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}