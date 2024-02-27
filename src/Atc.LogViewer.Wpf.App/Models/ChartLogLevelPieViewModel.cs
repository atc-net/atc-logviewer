namespace Atc.LogViewer.Wpf.App.Models;

public class ChartLogLevelPieViewModel : ViewModelBase
{
    private static readonly Dictionary<LogLevel, SolidColorPaint> LogLevelPaints = LogLevelFactory.CreatePaints();

    public ChartLogLevelPieViewModel()
    {
        Messenger.Default.Register<LogEntriesStatsMessage>(this, OnLogEntriesStatsMessageHandler);
    }

    public LabelVisual FilteredTitle { get; set; } =
        new()
        {
            Text = "Filtered Log Level Distribution",
            TextSize = 24,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DodgerBlue),
        };

    public LabelVisual TotalTitle { get; set; } =
        new()
        {
            Text = "Total Log Level Distribution",
            TextSize = 24,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DodgerBlue),
        };

    public IEnumerable<ISeries> FilteredSeries { get; set; } =
        new[]
        {
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Trace),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Trace],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Trace}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Debug),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Debug],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Debug}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Information),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Information],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Information}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Warning),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Warning],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Warning}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Error),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Error],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Error}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Critical),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Critical],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Critical}: {point.Coordinate.PrimaryValue}",
            },
        };

    public IEnumerable<ISeries> TotalSeries { get; set; } =
        new[]
        {
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Trace),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Trace],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Trace}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Debug),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Debug],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Debug}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Information),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Information],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Information}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Warning),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Warning],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Warning}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Error),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Error],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Error}: {point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Critical),
                Values = [0],
                IsVisible = false,
                Fill = LogLevelPaints[LogLevel.Critical],
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{LogLevel.Critical}: {point.Coordinate.PrimaryValue}",
            },
        };

    private void OnLogEntriesStatsMessageHandler(
            LogEntriesStatsMessage obj)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var traceFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Trace));
            traceFilteredSeries.IsVisible = obj.TraceCount > 0;
            traceFilteredSeries.Values = new[] { obj.TraceCount };

            var debugFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Debug));
            debugFilteredSeries.IsVisible = obj.DebugCount > 0;
            debugFilteredSeries.Values = new[] { obj.DebugCount };

            var informationFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Information));
            informationFilteredSeries.IsVisible = obj.InformationCount > 0;
            informationFilteredSeries.Values = new[] { obj.InformationCount };

            var warningFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Warning));
            warningFilteredSeries.IsVisible = obj.WarningCount > 0;
            warningFilteredSeries.Values = new[] { obj.WarningCount };

            var errorFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Error));
            errorFilteredSeries.IsVisible = obj.ErrorCount > 0;
            errorFilteredSeries.Values = new[] { obj.ErrorCount };

            var criticalFilteredSeries = FilteredSeries.First(x => x.Name == nameof(LogLevel.Critical));
            criticalFilteredSeries.IsVisible = obj.CriticalCount > 0;
            criticalFilteredSeries.Values = new[] { obj.CriticalCount };

            var traceTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Trace));
            traceTotalSeries.IsVisible = obj.TotalTraceCount > 0;
            traceTotalSeries.Values = new[] { obj.TotalTraceCount };

            var debugTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Debug));
            debugTotalSeries.IsVisible = obj.TotalDebugCount > 0;
            debugTotalSeries.Values = new[] { obj.TotalDebugCount };

            var informationTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Information));
            informationTotalSeries.IsVisible = obj.TotalInformationCount > 0;
            informationTotalSeries.Values = new[] { obj.TotalInformationCount };

            var warningTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Warning));
            warningTotalSeries.IsVisible = obj.TotalWarningCount > 0;
            warningTotalSeries.Values = new[] { obj.TotalWarningCount };

            var errorTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Error));
            errorTotalSeries.IsVisible = obj.TotalErrorCount > 0;
            errorTotalSeries.Values = new[] { obj.TotalErrorCount };

            var criticalTotalSeries = TotalSeries.First(x => x.Name == nameof(LogLevel.Critical));
            criticalTotalSeries.IsVisible = obj.TotalCriticalCount > 0;
            criticalTotalSeries.Values = new[] { obj.TotalCriticalCount };
        });
    }
}