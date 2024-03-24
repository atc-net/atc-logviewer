using Serilog;

namespace Atc.LogCollector.Tests.Serilog;

public class SerilogCollectorIntegrationTestBase : CollectorIntegrationTestBase
{
    private const int FileSizeLimit500Mb = 524_288_000;
    private Microsoft.Extensions.Logging.ILogger? logger;
    private FileInfo? logFile;

    public FileInfo CurrentLogFile => logFile ?? throw new InvalidOperationException("Log file has not been initialized.");

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        logFile = new FileInfo(Path.Combine(WorkingDirectory.FullName, $"serilog-test-{DateTime.Now:yyyyMMdd}.log"));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                path: logFile.FullName,
                rollingInterval: RollingInterval.Infinite,
                fileSizeLimitBytes: FileSizeLimit500Mb,
                rollOnFileSizeLimit: true,
                formatProvider: GlobalizationConstants.EnglishCultureInfo)
            .CreateLogger();

        var loggerFactory = new LoggerFactory()
            .AddSerilog();

        logger = loggerFactory
            .CreateLogger<SerilogCollectorIntegrationTestBase>();

        await Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        await Log.CloseAndFlushAsync();

        await base.DisposeAsync();
    }

    public void SendLogItemsToLogger()
        => SendLogItemsToLogger(logger!);

    public static async Task CloseAndFlushLogger()
        => await Log.CloseAndFlushAsync();
}