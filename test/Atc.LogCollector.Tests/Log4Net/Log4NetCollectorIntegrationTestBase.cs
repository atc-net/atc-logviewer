using log4net;
using log4net.Appender;
using log4net.Layout;

namespace Atc.LogCollector.Tests.Log4Net;

public class Log4NetCollectorIntegrationTestBase : CollectorIntegrationTestBase
{
    private const string FileSizeLimit500Mb = "500MB";
    private ILogger? logger;
    private FileInfo? logFile;

    public FileInfo CurrentLogFile => logFile ?? throw new InvalidOperationException("Log file has not been initialized.");

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        logFile = new FileInfo(Path.Combine(WorkingDirectory.FullName, $"log4net-test-{DateTime.Now:yyyyMMdd}.log"));

        var appender = new RollingFileAppender
        {
            File = logFile.FullName,
            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Size,
            MaxSizeRollBackups = 1,
            MaximumFileSize = FileSizeLimit500Mb,
            Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline"),
        };

        appender.ActivateOptions();

        var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
        hierarchy.Root.AddAppender(appender);
        hierarchy.Root.Level = log4net.Core.Level.Trace;
        hierarchy.Configured = true;

        var options = new Log4NetProviderOptions
        {
            ExternalConfigurationSetup = true,
        };

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddLog4Net(options);
        });

        logger = loggerFactory.CreateLogger<Log4NetCollectorIntegrationTestBase>();

        await Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        FlushAndShutdownLogger();

        return base.DisposeAsync();
    }

    public void SendLogItemsToLogger()
        => SendLogItemsToLogger(logger ?? throw new InvalidOperationException("Logger has not been initialized."));

    public static void FlushAndShutdownLogger()
    {
        LogManager.Flush(100);

        LogManager.ShutdownRepository();
        LogManager.Shutdown();
    }
}