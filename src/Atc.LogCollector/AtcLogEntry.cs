namespace Atc.LogCollector;

public record AtcLogEntry(
    string SourceIdentifier,
    long LineNumber,
    DateTime TimeStamp,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull);