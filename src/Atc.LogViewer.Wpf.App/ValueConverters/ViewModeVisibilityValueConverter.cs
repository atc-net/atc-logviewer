namespace Atc.LogViewer.Wpf.App.ValueConverters;

public class ViewModeVisibilityValueConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not ViewMode currentViewMode ||
            parameter is not ViewMode matchViewMode)
        {
            return Visibility.Collapsed;
        }

        return currentViewMode == matchViewMode
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) => throw new NotImplementedException();
}