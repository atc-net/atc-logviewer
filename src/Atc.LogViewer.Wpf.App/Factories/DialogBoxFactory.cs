namespace Atc.LogViewer.Wpf.App.Factories;

public static class DialogBoxFactory
{
    public static InputFormDialogBox CreateNewProfile()
    {
        var labelControlsLeftSide = BuildProfileLabelControlsLeftSide();
        var labelControlsRightSide = BuildProfileLabelControlsRightSide();

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControlsLeftSide);
        labelControlsForm.AddColumn(labelControlsRightSide);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            string.Empty,
            "Create new profile",
            labelControlsForm);

        return dialogBox;
    }

    public static InputFormDialogBox CreateEditProfile(
        ProfileViewModel profileViewModel)
    {
        ArgumentNullException.ThrowIfNull(profileViewModel);

        var labelControlsLeftSide = BuildProfileLabelControlsLeftSide();
        var labelControlsRightSide = BuildProfileLabelControlsRightSide();

        var lcName = labelControlsLeftSide.FindByIdentifier<LabelTextBox>(Constants.Forms.Profile.Name)!;
        lcName.IsEnabled = false;
        lcName.Text = profileViewModel.Name;

        var lcLogFolder = labelControlsLeftSide.FindByIdentifier<LabelDirectoryPicker>(Constants.Forms.Profile.LogFolder)!;
        lcLogFolder.Value = new DirectoryInfo(profileViewModel.LogFolder);

        var lcDefaultCollector = labelControlsLeftSide.FindByIdentifier<LabelComboBox>(Constants.Forms.Profile.Collector)!;
        lcDefaultCollector.SelectedKey = profileViewModel.CollectorType.ToString();

        var lcMaxDaysBack = labelControlsLeftSide.FindByIdentifier<LabelIntegerBox>(Constants.Forms.Profile.MaxDaysBack)!;
        lcMaxDaysBack.Value = profileViewModel.CollectorConfiguration.MaxDaysBack;

        var lcMonitorFiles = labelControlsLeftSide.FindByIdentifier<LabelToggleSwitch>(Constants.Forms.Profile.MonitorFiles)!;
        lcMonitorFiles.IsOn = profileViewModel.CollectorConfiguration.MonitorFiles;

        var lcFileNameTerms = labelControlsRightSide.FindByIdentifier<LabelContent>(Constants.Forms.Profile.FileNameTerms)!;
        var lcFileNameTermsViewModel = lcFileNameTerms.GetViewModel<TermsViewModel>()!;
        lcFileNameTermsViewModel.Terms.AddRange(profileViewModel.CollectorConfiguration.FileNameTerms);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControlsLeftSide);
        labelControlsForm.AddColumn(labelControlsRightSide);

        return new InputFormDialogBox(
            Application.Current.MainWindow!,
            string.Empty,
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

    private static List<ILabelControlBase> BuildProfileLabelControlsLeftSide()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = "Name",
                Tag = Constants.Forms.Profile.Name,
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelDirectoryPicker
            {
                LabelText = "Log folder",
                Tag = Constants.Forms.Profile.LogFolder,
                IsMandatory = true,
            },
            new LabelComboBox
            {
                LabelText = "Collector type",
                Tag = Constants.Forms.Profile.Collector,
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
                Tag = Constants.Forms.Profile.MaxDaysBack,
                Minimum = 0,
                Maximum = 365,
                Value = 10,
            },
            new LabelToggleSwitch
            {
                LabelText = "Monitor files",
                Tag = Constants.Forms.Profile.MonitorFiles,
                IsOn = true,
            },
        };

        return labelControls;
    }

    private static List<ILabelControlBase> BuildProfileLabelControlsRightSide()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelContent
            {
                LabelText = "File name terms",
                Tag = Constants.Forms.Profile.FileNameTerms,
                Content = new TermsView
                {
                    DataContext = new TermsViewModel(),
                },
            },
        };

        return labelControls;
    }
}