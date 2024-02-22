namespace Atc.LogAnalyzer;

public record LogFilter(
    bool LogLevelTrace,
    bool LogLevelDebug,
    bool LogLevelInfo,
    bool LogLevelWarning,
    bool LogLevelError,
    bool LogLevelCritical,
    string IncludeText,
    DateTime? StartTime,
    DateTime? EndTime);