namespace Atc.LogViewer.Wpf.App.Extractors;

public static class DialogBoxExtractor
{
    public static string ExtractProfileFileName(
        InputFormDialogBox dialogBox)
    {
        ArgumentNullException.ThrowIfNull(dialogBox);

        var data = dialogBox.Data.GetKeyValues();

        var name = data[Constants.Forms.Profile.Name].ToString()!;
        if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            name += ".json";
        }

        return name;
    }

    public static (ProfileViewModel? ProfileViewModel, string? WarningMessage) ExtractProfileViewModel(
        InputFormDialogBox dialogBox)
    {
        ArgumentNullException.ThrowIfNull(dialogBox);

        var data = dialogBox.Data.GetKeyValues();

        if (!Enum<LogFileCollectorType>.TryParse(
                data[Constants.Forms.Profile.Collector].ToString()!,
                ignoreCase: false,
                out var defaultCollectorType))
        {
            return (
                ProfileViewModel: null,
                WarningMessage: "Default Collector - was not selected");
        }

        var maxDaysBack = 0;
        if (NumberHelper.TryParseToInt(
                data[Constants.Forms.Profile.MaxDaysBack].ToString()!,
                out var maxDaysBackAsInt))
        {
            maxDaysBack = maxDaysBackAsInt;
        }

        var monitorFiles = false;
        if (bool.TryParse(
                data[Constants.Forms.Profile.MonitorFiles].ToString()!,
                out var monitorFilesResult))
        {
            monitorFiles = monitorFilesResult;
        }

        return (
            new ProfileViewModel
            {
                Name = data[Constants.Forms.Profile.Name].ToString()!,
                LogFolder = data[Constants.Forms.Profile.LogFolder].ToString()!,
                CollectorType = defaultCollectorType,
                CollectorConfiguration = new LogFileCollectorConfiguration
                {
                    MaxDaysBack = (ushort)maxDaysBack,
                    FileNameTerms = new List<string>(),
                    MonitorFiles = monitorFiles,
                },
            },
            WarningMessage: null);
    }
}