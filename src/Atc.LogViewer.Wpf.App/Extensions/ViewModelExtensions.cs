namespace Atc.LogViewer.Wpf.App.Extensions;

public static class ViewModelExtensions
{
    public static HighlightOptions ToOptions(
        this HighlightViewModel highlightViewModel)
    {
        ArgumentNullException.ThrowIfNull(highlightViewModel);

        return new HighlightOptions
        {
            Text = highlightViewModel.Text,
            IgnoreCasing = highlightViewModel.IgnoreCasing,
            Foreground = highlightViewModel.Foreground.ToString(GlobalizationConstants.EnglishCultureInfo),
            Background = highlightViewModel.Background.ToString(GlobalizationConstants.EnglishCultureInfo),
        };
    }

    public static IList<HighlightOptions> ToViewModels(
        this ObservableCollectionEx<HighlightViewModel> highlightsViewModels)
    {
        ArgumentNullException.ThrowIfNull(highlightsViewModels);

        return highlightsViewModels
            .Select(x => x.ToOptions())
            .ToList();
    }

    public static ProfileOptions ToOptions(
        this ProfileViewModel profileViewModel)
    {
        ArgumentNullException.ThrowIfNull(profileViewModel);

        return new ProfileOptions
        {
            DefaultLogFolder = profileViewModel.DefaultLogFolder,
            DefaultCollectorType = profileViewModel.DefaultCollectorType,
            Highlights = profileViewModel.Highlights.ToViewModels(),
        };
    }
}