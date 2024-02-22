namespace Atc.LogCollector;

public record AtcLogEntry(
    string SourceIdentifier,
    DateTime TimeStamp,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull);