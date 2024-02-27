namespace Atc.LogViewer.Wpf.App.Models;

public enum ChartTimelineMode
{
    [Description("Latest 12 Hours")]
    Latest12Hours,

    [Description("Latest 24 Hours")]
    Latest24Hours,

    [Description("Latest 48 Hours")]
    Latest48Hours,

    [Description("Latest 60 Minutes")]
    Latest60Minutes,
}