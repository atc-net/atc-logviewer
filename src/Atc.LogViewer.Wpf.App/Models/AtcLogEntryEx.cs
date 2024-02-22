namespace Atc.LogViewer.Wpf.App.Models;

public record AtcLogEntryEx(
    string SourceIdentifier,
    DateTime DateTime,
    LogLevel LogLevel,
    string MessageShort,
    string MessageFull,
    Brush HighlightForeground,
    Brush HighlightBackground)
    : AtcLogEntry(
        SourceIdentifier,
        DateTime,
        LogLevel,
        MessageShort,
        MessageFull)
{
}