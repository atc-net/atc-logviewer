namespace Atc.LogViewer.Wpf.App.Factories;

public static class DialogBoxFactory
{
    public static InputFormDialogBox CreateNewProfile()
    {
        var labelControls = BuildProfileLabelControls();
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        return new InputFormDialogBox(
            Application.Current.MainWindow!,
            "Create new profile",
            labelControlsForm);
    }

    public static InputFormDialogBox CreateEditProfile(
        ProfileViewModel profileViewModel)
    {
        ArgumentNullException.ThrowIfNull(profileViewModel);

        var labelControls = BuildProfileLabelControls();

        var labelControlName = (LabelTextBox)labelControls.Find(x => x.Identifier == Constance.Forms.Profile.Name)!;
        labelControlName.IsEnabled = false;
        labelControlName.Text = profileViewModel.Name;

        var lcLogFolder = (LabelDirectoryPicker)labelControls.Find(x => x.Identifier == Constance.Forms.Profile.LogFolder)!;
        lcLogFolder.Value = new DirectoryInfo(profileViewModel.LogFolder);

        var lcDefaultCollector = (LabelComboBox)labelControls.Find(x => x.Identifier == Constance.Forms.Profile.Collector)!;
        lcDefaultCollector.SelectedKey = profileViewModel.CollectorType.ToString();

        var lcMaxDaysBack = (LabelIntegerBox)labelControls.Find(x => x.Identifier == Constance.Forms.Profile.MaxDaysBack)!;
        lcMaxDaysBack.Value = profileViewModel.CollectorConfiguration.MaxDaysBack;

        var lcMonitorFiles = (LabelCheckBox)labelControls.Find(x => x.Identifier == Constance.Forms.Profile.MonitorFiles)!;
        lcMonitorFiles.IsChecked = profileViewModel.CollectorConfiguration.MonitorFiles;

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        return new InputFormDialogBox(
            Application.Current.MainWindow!,
            "Edit profile",
            labelControlsForm);
    }

    public static InfoDialogBox CreateWarningOption(
        string message)
        => new(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Warning)
            {
                TitleBarText = "Invalid options",
            },
            message);

    public static InfoDialogBox CreateWarningHighlight()
        => new(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Warning)
            {
                TitleBarText = "Invalid highlight",
            },
            "All highlight must have a text");

    private static List<ILabelControlBase> BuildProfileLabelControls()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = "Name",
                Tag = Constance.Forms.Profile.Name,
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelDirectoryPicker
            {
                LabelText = "Log folder",
                Tag = Constance.Forms.Profile.LogFolder,
                IsMandatory = true,
            },
            new LabelComboBox
            {
                LabelText = "Collector type",
                Tag = Constance.Forms.Profile.Collector,
                IsMandatory = true,
                SelectedKey = nameof(DropDownFirstItemType.PleaseSelect),
                Items = Enum<LogFileCollectorType>.ToDictionaryWithStringKey(
                    DropDownFirstItemType.PleaseSelect,
                    useDescriptionAttribute: true,
                    includeDefault: false,
                    SortDirectionType.Ascending),
            },
            new LabelIntegerBox
            {
                LabelText = "Max days back",
                Tag = Constance.Forms.Profile.MaxDaysBack,
                Minimum = 0,
                Maximum = 365,
                Value = 10,
            },
            new LabelCheckBox
            {
                LabelText = "Monitor files",
                Tag = Constance.Forms.Profile.MonitorFiles,
                IsChecked = true,
            },
        };

        return labelControls;
    }
}