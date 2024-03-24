using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = NLog.LogLevel;

// ReSharper disable StringLiteralTypo
namespace Atc.LogCollector.Tests.NLog;

public class NLogCollectorIntegrationTestBase : CollectorIntegrationTestBase
{
    private const int FileSizeLimit500Mb = 524_288_000;
    private Microsoft.Extensions.Logging.ILogger? logger;
    private FileInfo? logFile;

    public FileInfo CurrentLogFile => logFile ?? throw new InvalidOperationException("Log file has not been initialized.");

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        logFile = new FileInfo(Path.Combine(WorkingDirectory.FullName, $"nlog-test-{DateTime.Now:yyyyMMdd}.log"));

        var config = new LoggingConfiguration();
        var fileTarget = new FileTarget
        {
            Name = "FileTarget",
            FileName = logFile.FullName,
            ArchiveAboveSize = FileSizeLimit500Mb,
            ArchiveNumbering = ArchiveNumberingMode.Rolling,
            MaxArchiveFiles = 1,
            Layout = "${longdate} ${uppercase:${level}} ${message}",
        };

        config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);

        LogManager.Configuration = config;

#pragma warning disable CS0618 // Type or member is obsolete
        var factory = new LoggerFactory()
            .AddNLog();
#pragma warning restore CS0618 // Type or member is obsolete

        logger = factory
            .CreateLogger<NLogCollectorIntegrationTestBase>();

        await Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        LogManager.Shutdown();

        return base.DisposeAsync();
    }

    public void SendLogItemsToLogger()
        => SendLogItemsToLogger(logger ?? throw new InvalidOperationException("Logger has not been initialized."));

    public static void FlushAndShutdownLogger()
    {
        LogManager.Flush();

        LogManager.Shutdown();
    }
}