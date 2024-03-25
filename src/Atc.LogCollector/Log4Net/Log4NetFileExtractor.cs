namespace Atc.LogCollector.Log4Net;

public class Log4NetFileExtractor : ILog4NetFileExtractor
{
    public AtcLogEntry? ParseRootLine(
        string sourceIdentifier,
        string sourceSystem,
        long lineNumber,
        string line)
    {
        //// Based on: ConversionPattern = "%date [%thread] %-5level %logger - %message%newline",

        if (line is null ||
            line.Length < 36 ||
            !char.IsDigit(line[0]))
        {
            return null;
        }

        try
        {
            var threadStartIndex = line.IndexOf('[', StringComparison.Ordinal) + 1;
            var threadEndIndex = line.IndexOf(']', threadStartIndex);
            if (threadStartIndex == 0 || threadEndIndex == -1)
            {
                return null;
            }

            var dateTimeString = line[..(threadStartIndex - 2)];
            var dateTime = DateTime.Parse(dateTimeString, GlobalizationConstants.EnglishCultureInfo);

            var lineAfterThread = line[(threadEndIndex + 2)..];
            var logLevelEndIndex = lineAfterThread.IndexOf(' ', StringComparison.Ordinal);
            var logLevelString = lineAfterThread[..logLevelEndIndex];

            var logLevel = logLevelString switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Information,
                "WARN" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "FATAL" => LogLevel.Critical,
                _ => LogLevel.None,
            };

            if (logLevel == LogLevel.None)
            {
                return null;
            }

            var messageStartIndex = lineAfterThread.IndexOf('-', StringComparison.Ordinal) + 2;
            if (messageStartIndex < 3)
            {
                return null;
            }

            var message = lineAfterThread[messageStartIndex..];

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