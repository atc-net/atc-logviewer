namespace Atc.LogViewer.Wpf.App.Models;

public record AtcLogEntryEx(
    string SourceIdentifier,
    long LineNumber,
    DateTime TimeStamp,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull,
    Brush HighlightForeground,
    Brush HighlightBackground)
    : AtcLogEntry(
        SourceIdentifier,
        LineNumber,
        TimeStamp,
        LogLevel,
        MessageShort,
        MessageFull);