namespace Atc.LogViewer.Wpf.App.Models;

public record AtcLogEntryEx(
    string SourceIdentifier,
    DateTime TimeStamp,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull,
    Brush HighlightForeground,
    Brush HighlightBackground)
    : AtcLogEntry(
        SourceIdentifier,
        TimeStamp,
        LogLevel,
        MessageShort,
        MessageFull);