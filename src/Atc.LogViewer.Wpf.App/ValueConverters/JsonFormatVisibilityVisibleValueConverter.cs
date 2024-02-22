namespace Atc.LogViewer.Wpf.App.ValueConverters;

public class JsonFormatVisibilityVisibleValueConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var isJson = value is string stringValue &&
                     stringValue.IsFormatJson();

        if (parameter is null || (parameter is string param &&
                                  bool.TryParse(param, out var result) &&
                                  result))
        {
            return isJson
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        return isJson
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}