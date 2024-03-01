namespace Atc.LogCollector.Tail;

public record TailLine(
    FileInfo File,
    long LineNumber,
    string Line);