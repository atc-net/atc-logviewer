namespace Atc.LogCollector.Serilog;

public sealed class SerilogFileExtractor : ISerilogFileExtractor
{
    public AtcLogEntry? ParseRootLine(
        string sourceIdentifier,
        string sourceSystem,
        long lineNumber,
        string line)
    {
        //// layout = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}",

        if (line is null ||
            line.Length < 38 ||
            !char.IsDigit(line[0]))
        {
            return null;
        }

        try
        {
            var indexOfLogLevelStart = line.IndexOf('[', StringComparison.Ordinal) + 1;
            var indexOfLogLevelEnd = line.IndexOf(']', indexOfLogLevelStart);

            var dateTimeString = line[..(indexOfLogLevelStart - 2)];
            var dateTime = DateTime.Parse(dateTimeString, GlobalizationConstants.EnglishCultureInfo);

            var logLevelString = line[indexOfLogLevelStart..indexOfLogLevelEnd];
            var logLevel = logLevelString switch
            {
                "VRB" => LogLevel.Trace,
                "DBG" => LogLevel.Debug,
                "INF" => LogLevel.Information,
                "WRN" => LogLevel.Warning,
                "ERR" => LogLevel.Error,
                "FTL" => LogLevel.Critical,
                _ => LogLevel.None,
            };

            if (logLevel == LogLevel.None)
            {
                return null;
            }

            var message = line[(indexOfLogLevelEnd + 2)..];

            return new AtcLogEntry(
                sourceIdentifier,
                sourceSystem,
                lineNumber,
                dateTime,
                logLevel,
                message,
                message);
        }
        catch
        {
            return null;
        }
    }

    public bool HasSubLines(
        string[] lines,
        int lineNumber)
        => lines is not null &&
           lineNumber != lines.Length - 1 &&
           lines[lineNumber + 1].Length > 5 &&
           !char.IsDigit(lines[lineNumber + 1][0]);

    public string GetMessageWithSubLines(
        string rootMessage,
        string[] lines,
        ref int lineNumber)
    {
        if (rootMessage is null ||
            lines is null)
        {
            return string.Empty;
        }

        var errorSubLineCount = 0;
        for (var subLineNumber = 1; subLineNumber < 120; subLineNumber++)
        {
            var lineOffset = lineNumber + subLineNumber;
            if (lineOffset >= lines.Length - 1)
            {
                break;
            }

            var subLine = lines[lineOffset];
            if (subLine.Length == 0)
            {
                continue;
            }

            if (char.IsDigit(subLine[0]))
            {
                errorSubLineCount--;
                break;
            }

            errorSubLineCount = subLineNumber + 1;
        }

        var sb = new StringBuilder();
        sb.AppendLine(rootMessage);

        for (var j = 0; j < errorSubLineCount; j++)
        {
            lineNumber++;
            if (j == errorSubLineCount - 1)
            {
                sb.Append(lines[lineNumber]);
            }
            else
            {
                sb.AppendLine(lines[lineNumber]);
            }
        }

        return sb.ToString();
    }
}