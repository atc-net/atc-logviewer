namespace Atc.LogCollector;

public record AtcLogEntry(
    string SourceIdentifier,
    string SourceSystem,
    long LineNumber,
    DateTime TimeStamp,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull);