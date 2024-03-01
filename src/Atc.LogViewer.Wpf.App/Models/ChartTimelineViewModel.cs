namespace Atc.LogViewer.Wpf.App.Models;

public class ChartTimelineViewModel : ViewModelBase
{
    private static readonly LogLevel[] LogLevels = LogLevelFactory.CreateList();
    private static readonly Dictionary<LogLevel, SKColor> LogLevelColors = LogLevelFactory.CreateSkColors();
    private readonly ObservableCollectionEx<AtcLogEntryEx> logEntries;
    private IDictionary<string, string> intervals = new Dictionary<string, string>(StringComparer.Ordinal);
    private string selectedInterval = string.Empty;
    private ISeries<long>[] series = [];
    private Axis[] xAxis = [];

    public ChartTimelineViewModel(
        ObservableCollectionEx<AtcLogEntryEx> logEntries)
    {
        ArgumentNullException.ThrowIfNull(logEntries);

        this.logEntries = logEntries;

        Intervals = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { nameof(ChartTimelineMode.Latest60Minutes), ChartTimelineMode.Latest60Minutes.GetDescription() },
            { nameof(ChartTimelineMode.Latest12Hours), ChartTimelineMode.Latest12Hours.GetDescription() },
            { nameof(ChartTimelineMode.Latest24Hours), ChartTimelineMode.Latest24Hours.GetDescription() },
            { nameof(ChartTimelineMode.Latest48Hours), ChartTimelineMode.Latest48Hours.GetDescription() },
        };

        SelectedInterval = Intervals.First().Key;
    }

    public IRelayCommand RefreshCommand => new RelayCommand(RefreshCommandHandler);

    public IDictionary<string, string> Intervals
    {
        get => intervals;
        set
        {
            intervals = value;
            OnPropertyChanged();
        }
    }

    public string SelectedInterval
    {
        get => selectedInterval;
        set
        {
            selectedInterval = value;
            RaisePropertyChanged();

            RefreshChart();
        }
    }

    public LabelVisual Title { get; set; } =
        new()
        {
            Text = "Timeline Distribution",
            TextSize = 24,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DodgerBlue),
        };

    public ISeries<long>[] Series
    {
        get => series;
        set
        {
            series = value;
            RaisePropertyChanged();
        }
    }

    public Axis[] XAxis
    {
        get => xAxis;
        set
        {
            if (Equals(value, xAxis))
            {
                return;
            }

            xAxis = value;
            RaisePropertyChanged();
        }
    }

    public Axis[] YAxis { get; set; } =
    [
        new Axis
        {
            MinLimit = 0,
            MinStep = 1,
        },
    ];

    public void RefreshChart()
    {
        switch (SelectedInterval)
        {
            case nameof(ChartTimelineMode.Latest12Hours):
                PopulateSeriesLatestHours(12);
                break;
            case nameof(ChartTimelineMode.Latest24Hours):
                PopulateSeriesLatestHours(24);
                break;
            case nameof(ChartTimelineMode.Latest48Hours):
                PopulateSeriesLatestHours(48);
                break;
            case nameof(ChartTimelineMode.Latest60Minutes):
                PopulateSeriesLatest60Minutes();
                break;
        }
    }

    private void RefreshCommandHandler()
        => RefreshChart();

    public void PopulateSeriesLatestHours(
        int latestHours)
    {
        var seriesCollection = new List<StackedColumnSeries<long>>();

        var groupedByHour = logEntries
            .Where(entry => entry.TimeStamp > DateTime.Now.AddHours(-latestHours))
            .GroupBy(entry => new { entry.TimeStamp.Hour, entry.LogLevel })
            .Select(group => new
            {
                group.Key.Hour,
                group.Key.LogLevel,
                Count = group.Count(),
            })
            .ToList();

        var now = DateTime.Now;
        var startHour = now.AddHours(-latestHours + 1).Hour;

        foreach (var logLevel in LogLevels)
        {
            var values = new long[latestHours];
            for (var i = 0; i < latestHours; i++)
            {
                var targetHour = (startHour + i) % 24;
                var count = groupedByHour.Find(x => x.Hour == targetHour && x.LogLevel == logLevel)?.Count ?? 0;
                values[i] = count;
            }

            var stackedColumnSeries = new StackedColumnSeries<long>
            {
                Name = logLevel.ToString(),
                Values = values,
                Fill = new SolidColorPaint(LogLevelColors[logLevel]),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle,
                DataLabelsRotation = -90,
                DataLabelsFormatter = point
                    => point.Coordinate.PrimaryValue == 0
                        ? string.Empty
                        : $"{logLevel}: {point.Coordinate.PrimaryValue}",
            };

            seriesCollection.Add(stackedColumnSeries);
        }

        // ReSharper disable once CoVariantArrayConversion
        Series = seriesCollection.ToArray();

        var labels = new string[latestHours];
        for (var i = 0; i < latestHours; i++)
        {
            var hour = (startHour + i) % 24;
            labels[i] = $"{hour}:00";
        }

        XAxis =
        [
            new Axis
            {
                LabelsRotation = -90,
                Labels = labels,
            },
        ];
    }

    public void PopulateSeriesLatest60Minutes()
    {
        var seriesCollection = new List<StackedColumnSeries<long>>();

        var groupedByMinute = logEntries
            .Where(entry => entry.TimeStamp > DateTime.Now.AddMinutes(-60))
            .GroupBy(entry => new { entry.TimeStamp.Hour, entry.TimeStamp.Minute, entry.LogLevel })
            .Select(group => new
            {
                group.Key.Hour,
                group.Key.Minute,
                group.Key.LogLevel,
                Count = group.Count(),
            })
            .ToList();

        var now = DateTime.Now;
        var startMinute = now.AddMinutes(-59).Minute;
        var startHour = now.AddMinutes(-59).Hour;

        foreach (var logLevel in LogLevels)
        {
            var values = new long[60];
            for (var i = 0; i < 60; i++)
            {
                var targetMinute = (startMinute + i) % 60;
                var targetHour = (startHour + ((startMinute + i) / 60)) % 24;
                var count = groupedByMinute.Find(x => x.Minute == targetMinute && x.Hour == targetHour && x.LogLevel == logLevel)?.Count ?? 0;
                values[i] = count;
            }

            var stackedColumnSeries = new StackedColumnSeries<long>
            {
                Name = logLevel.ToString(),
                Values = values,
                Fill = new SolidColorPaint(LogLevelColors[logLevel]),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle,
                DataLabelsRotation = -90,
                DataLabelsFormatter = point => point.Coordinate.PrimaryValue == 0 ? string.Empty : $"{logLevel}: {point.Coordinate.PrimaryValue}",
            };

            seriesCollection.Add(stackedColumnSeries);
        }

        // ReSharper disable once CoVariantArrayConversion
        Series = seriesCollection.ToArray();

        var labels = new string[60];
        for (var i = 0; i < 60; i++)
        {
            var minute = (startMinute + i) % 60;
            var hour = (startHour + ((startMinute + i) / 60)) % 24;
            labels[i] = $"{hour:D2}:{minute:D2}";
        }

        XAxis =
        [
            new Axis
            {
                LabelsRotation = -90,
                Labels = labels,
            },
        ];
    }
}