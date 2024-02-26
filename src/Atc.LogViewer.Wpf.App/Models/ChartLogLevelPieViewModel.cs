namespace Atc.LogViewer.Wpf.App.Models;

public class ChartLogLevelPieViewModel : ViewModelBase
{
    public ChartLogLevelPieViewModel()
    {
        Messenger.Default.Register<LogEntriesStatsMessage>(this, OnLogEntriesStatsMessageHandler);
    }

    public LabelVisual Title { get; set; } =
        new()
        {
            Text = "Log Level Distribution",
            TextSize = 24,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DodgerBlue),
        };

    public IEnumerable<ISeries> Series { get; set; } =
        new[]
        {
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Trace),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.Gray),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Debug),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.CadetBlue),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Information),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.DodgerBlue),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Warning),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.Goldenrod),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Error),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.Crimson),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
            new PieSeries<long>
            {
                Name = nameof(LogLevel.Critical),
                Values = [0],
                IsVisible = false,
                Fill = new SolidColorPaint(SKColors.Red),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsSize = 18,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}",
            },
        };

    private void OnLogEntriesStatsMessageHandler(
            LogEntriesStatsMessage obj)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var traceSeries = Series.First(x => x.Name == nameof(LogLevel.Trace));
            traceSeries.IsVisible = obj.TraceCount > 0;
            traceSeries.Values = new[] { obj.TraceCount };

            var debugSeries = Series.First(x => x.Name == nameof(LogLevel.Debug));
            debugSeries.IsVisible = obj.DebugCount > 0;
            debugSeries.Values = new[] { obj.DebugCount };

            var informationSeries = Series.First(x => x.Name == nameof(LogLevel.Information));
            informationSeries.IsVisible = obj.InformationCount > 0;
            informationSeries.Values = new[] { obj.InformationCount };

            var warningSeries = Series.First(x => x.Name == nameof(LogLevel.Warning));
            warningSeries.IsVisible = obj.WarningCount > 0;
            warningSeries.Values = new[] { obj.WarningCount };

            var errorSeries = Series.First(x => x.Name == nameof(LogLevel.Error));
            errorSeries.IsVisible = obj.ErrorCount > 0;
            errorSeries.Values = new[] { obj.ErrorCount };

            var criticalSeries = Series.First(x => x.Name == nameof(LogLevel.Critical));
            criticalSeries.IsVisible = obj.CriticalCount > 0;
            criticalSeries.Values = new[] { obj.CriticalCount };
        });
    }
}