namespace Atc.LogCollector.Serilog;

public interface ISerilogFileExtractor
{
    AtcLogEntry? ParseRootLine(
        string sourceIdentifier,
        string line);

    bool HasSubLines(
        string[] lines,
        int lineNumber);

    string GetMessageWithSubLines(
        string rootMessage,
        string[] lines,
        ref int lineNumber);
}