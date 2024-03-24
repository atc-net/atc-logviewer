namespace Atc.LogCollector.NLog;

public sealed class NLogFileExtractor : INLogFileExtractor
{
    public AtcLogEntry? ParseRootLine(
        string sourceIdentifier,
        string sourceSystem,
        long lineNumber,
        string line)
    {
        if (line is null ||
            line.Length < 25 ||
            !char.IsDigit(line[0]))
        {
            return null;
        }

        try
        {
            var dateTimeString = line[..23];
            var dateTime = DateTime.Parse(dateTimeString, GlobalizationConstants.EnglishCultureInfo);
            var indexOfLogLevelStart = line.IndexOf(' ', 24) + 1;
            var indexOfLogLevelEnd = line.IndexOf(' ', indexOfLogLevelStart);
            if (indexOfLogLevelStart < 0 || indexOfLogLevelStart >= line.Length)
            {
                return null;
            }

            var logLevelString = line[indexOfLogLevelStart..indexOfLogLevelEnd];
            var message = line[(indexOfLogLevelEnd + 1)..];

            var logLevel = logLevelString switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Information,
                "WARN" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "FATAL" => LogLevel.Critical,
                _ => throw new SwitchCaseDefaultException($"Unknown log level: {logLevelString}"),
            };

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