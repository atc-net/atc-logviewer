// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.LogCollector.Tests;

public class CollectorIntegrationTestBase : IAsyncLifetime
{
    internal static readonly DirectoryInfo WorkingDirectory = new(
        Path.Combine(Path.GetTempPath(), "atc-integration-test-log-collector"));

    public virtual Task InitializeAsync()
    {
        if (Directory.Exists(WorkingDirectory.FullName))
        {
            Directory.Delete(WorkingDirectory.FullName, recursive: true);
        }

        Directory.CreateDirectory(WorkingDirectory.FullName);

        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        if (!Directory.Exists(WorkingDirectory.FullName))
        {
            return Task.CompletedTask;
        }

        try
        {
            Directory.Delete(WorkingDirectory.FullName, recursive: true);
        }
        catch
        {
            // Dummy
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<LogItem> LogItems { get; } =
    [
        LogItemFactory.CreateInformation("App started"),
        LogItemFactory.CreateTrace("Log trace"),
        LogItemFactory.CreateDebug("Log debug"),
        LogItemFactory.CreateInformation("Log information"),
        LogItemFactory.CreateWarning("Log warning"),
        LogItemFactory.CreateError("Log error"),
        LogItemFactory.CreateCritical("Log critical"),
        LogItemFactory.CreateInformation("App ended"),
    ];

    public void SendLogItemsToLogger(
        ILogger logger)
    {
        foreach (var logItem in LogItems)
        {
            switch (logItem.Severity)
            {
                case LogCategoryType.Critical:
                    logger.LogCritical(logItem.Message);
                    break;
                case LogCategoryType.Error:
                    logger.LogError(logItem.Message);
                    break;
                case LogCategoryType.Warning:
                    logger.LogWarning(logItem.Message);
                    break;
                case LogCategoryType.Information:
                    logger.LogInformation(logItem.Message);
                    break;
                case LogCategoryType.Debug:
                    logger.LogDebug(logItem.Message);
                    break;
                case LogCategoryType.Trace:
                    logger.LogTrace(logItem.Message);
                    break;
            }
        }
    }
}