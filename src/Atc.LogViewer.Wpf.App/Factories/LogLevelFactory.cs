namespace Atc.LogViewer.Wpf.App.Factories;

public static class LogLevelFactory
{
    public static LogLevel[] CreateList()
        => Enum.GetValues(typeof(LogLevel))
            .Cast<LogLevel>()
            .Where(level => level != LogLevel.None)
            .ToArray();

    [SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "OK.")]
    public static Dictionary<LogLevel, SKColor> CreateSkColors()
        => new()
        {
            { LogLevel.Critical, SKColors.Red },
            { LogLevel.Error, SKColors.Crimson },
            { LogLevel.Warning, SKColors.Goldenrod },
            { LogLevel.Information, SKColors.DodgerBlue },
            { LogLevel.Debug, SKColors.CadetBlue },
            { LogLevel.Trace, SKColors.Gray },
        };

    [SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "OK.")]
    public static Dictionary<LogLevel, SolidColorPaint> CreatePaints()
        => new()
        {
            { LogLevel.Critical, new SolidColorPaint(SKColors.Red) },
            { LogLevel.Error, new SolidColorPaint(SKColors.Crimson) },
            { LogLevel.Warning, new SolidColorPaint(SKColors.Goldenrod) },
            { LogLevel.Information, new SolidColorPaint(SKColors.DodgerBlue) },
            { LogLevel.Debug, new SolidColorPaint(SKColors.CadetBlue) },
            { LogLevel.Trace, new SolidColorPaint(SKColors.Gray) },
        };
}