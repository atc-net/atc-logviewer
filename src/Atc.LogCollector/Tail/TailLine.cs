namespace Atc.LogCollector.Tail;

public record TailLine(
    FileInfo File,
    string Line);