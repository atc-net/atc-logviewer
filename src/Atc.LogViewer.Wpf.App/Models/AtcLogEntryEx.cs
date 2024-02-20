namespace Atc.LogViewer.Wpf.App.Models;

public record AtcLogEntryEx(
    string SourceIdentifier,
    DateTime DateTime,
    LogLevel LogLevel,
    string Message,
    Brush HighlightForeground,
    Brush HighlightBackground)
    : AtcLogEntry(SourceIdentifier, DateTime, LogLevel, Message)
{
}