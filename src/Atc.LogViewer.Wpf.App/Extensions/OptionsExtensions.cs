namespace Atc.LogViewer.Wpf.App.Extensions;

public static class OptionsExtensions
{
    public static HighlightViewModel ToViewModel(
        this HighlightOptions highlightOptions)
    {
        ArgumentNullException.ThrowIfNull(highlightOptions);

        return new HighlightViewModel
        {
            Text = highlightOptions.Text,
            IgnoreCasing = highlightOptions.IgnoreCasing,
            Foreground = SolidColorBrushHelper.GetBrushFromHex(highlightOptions.Foreground)!,
            Background = SolidColorBrushHelper.GetBrushFromHex(highlightOptions.Background)!,
        };
    }

    public static ObservableCollectionEx<HighlightViewModel> ToViewModels(
        this IList<HighlightOptions> highlightsOptions)
    {
        ArgumentNullException.ThrowIfNull(highlightsOptions);

        var list = new ObservableCollectionEx<HighlightViewModel>();
        foreach (var highlightsOption in highlightsOptions)
        {
            list.Add(highlightsOption.ToViewModel());
        }

        return list;
    }

    public static ProfileViewModel ToViewModel(
        this ProfileOptions profileOptions)
    {
        ArgumentNullException.ThrowIfNull(profileOptions);

        return new ProfileViewModel
        {
            Name = profileOptions.Name,
            LogFolder = profileOptions.LogFolder,
            CollectorType = profileOptions.CollectorType,
            Highlights = profileOptions.Highlights.ToViewModels(),
            CollectorConfiguration = profileOptions.CollectorConfiguration,
        };
    }
}