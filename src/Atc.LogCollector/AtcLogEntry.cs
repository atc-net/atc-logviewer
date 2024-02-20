namespace Atc.LogCollector;

public record AtcLogEntry(
    string SourceIdentifier,
    DateTime DateTime,
    LogLevel LogLevel,
    string Message);