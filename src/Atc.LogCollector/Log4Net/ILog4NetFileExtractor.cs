namespace Atc.LogCollector.Log4Net;

public interface ILog4NetFileExtractor
{
    AtcLogEntry? ParseRootLine(
        string sourceIdentifier,
        string sourceSystem,
        long lineNumber,
        string line);

    bool HasSubLines(
        string[] lines,
        int lineNumber);

    string GetMessageWithSubLines(
        string rootMessage,
        string[] lines,
        ref int lineNumber);
}