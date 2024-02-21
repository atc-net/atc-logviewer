namespace Atc.LogViewer.Wpf.App.Factories;

public static class DialogBoxFactory
{
    public static InputFormDialogBox CreateNewProfile()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = "Name",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelDirectoryPicker
            {
                LabelText = "Default Log Folder",
                IsMandatory = true,
            },
            new LabelComboBox
            {
                LabelText = "Default Collector",
                IsMandatory = true,
                SelectedKey = nameof(DropDownFirstItemType.PleaseSelect),
                Items = Enum<LogFileCollectorType>.ToDictionaryWithStringKey(
                    DropDownFirstItemType.PleaseSelect,
                    useDescriptionAttribute: true,
                    includeDefault: false,
                    SortDirectionType.Ascending),
            },
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        return new InputFormDialogBox(
            Application.Current.MainWindow!,
            "Create new profile",
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
}