namespace Atc.LogViewer.Wpf.App.Models;

public class RecentOpenFileViewModel : ViewModelBase
{
    private readonly DirectoryInfo logViewerProgramDataProjectsDirectory;

    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    public RecentOpenFileViewModel()
    {
        logViewerProgramDataProjectsDirectory = new DirectoryInfo(@"C:\");
    }

    public RecentOpenFileViewModel(
        DirectoryInfo logViewerProgramDataProjectsDirectory,
        DateTime timeStamp,
        string file)
    {
        ArgumentNullException.ThrowIfNull(logViewerProgramDataProjectsDirectory);
        ArgumentNullException.ThrowIfNull(file);

        this.logViewerProgramDataProjectsDirectory = logViewerProgramDataProjectsDirectory;
        TimeStamp = timeStamp;
        File = file;
    }

    private DateTime timeStamp;
    private string file = string.Empty;

    public DateTime TimeStamp
    {
        get => timeStamp;
        set
        {
            timeStamp = value;
            RaisePropertyChanged();
        }
    }

    public string File
    {
        get => file;
        set
        {
            file = value;
            RaisePropertyChanged();
        }
    }

    public string FileDisplay
    {
        get
        {
            if (!file.StartsWith(logViewerProgramDataProjectsDirectory.FullName, StringComparison.Ordinal))
            {
                return file;
            }

            var fileInfo = new FileInfo(file);
            return $"Profile: {fileInfo.Directory!.Name} - {fileInfo.Name}";
        }
    }

    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(File)}: {File}, {nameof(FileDisplay)}: {FileDisplay}";
}