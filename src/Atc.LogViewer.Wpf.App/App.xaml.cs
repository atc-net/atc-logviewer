// ReSharper disable NotAccessedField.Local
namespace Atc.LogViewer.Wpf.App;

/// <summary>
/// Interaction logic for App.
/// </summary>
public partial class App
{
    private readonly ILogger<App> logger;
    private readonly IHost host;
    private IConfiguration? configuration;

    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    public static readonly BitmapImage DefaultIcon = new(new Uri("pack://application:,,,/Resources/AppIcon.ico", UriKind.Absolute));

    public static DirectoryInfo LogViewerCommonApplicationDataDirectory => new(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ATC"), "atc-logviewer"));

    public static DirectoryInfo LogViewerProgramDataRecentProfilesDirectory => LogViewerCommonApplicationDataDirectory;

    public static DirectoryInfo LogViewerProgramDataProfilesDirectory => new(Path.Combine(LogViewerCommonApplicationDataDirectory.FullName, "Profiles"));

    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
            jsonSerializerOptions.PropertyNamingPolicy = null;
            jsonSerializerOptions.Converters.Add(new JsonSolidColorBrushToHexConverter());
            return jsonSerializerOptions;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        RestoreInstallerCustomAppSettingsIfNeeded();

        host = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, logging) =>
            {
                logging
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddDebug();
            })
            .ConfigureAppConfiguration(
                configurationBuilder =>
                {
                    configuration = configurationBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(AtcFileNameConstants.AppSettings, optional: false, reloadOnChange: true)
                        .AddJsonFile(AtcFileNameConstants.AppSettingsCustom, optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
            .ConfigureServices((_, services) =>
            {
                services
                    .AddOptions<BasicApplicationOptions>()
                    .Bind(configuration!.GetRequiredSection(BasicApplicationOptions.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.AddSingleton<ILog4NetFileExtractor, Log4NetFileExtractor>();
                services.AddSingleton<ILog4NetFileCollector, Log4NetFileCollector>();

                services.AddSingleton<INLogFileExtractor, NLogFileExtractor>();
                services.AddSingleton<INLogFileCollector, NLogFileCollector>();

                services.AddSingleton<ISerilogFileExtractor, SerilogFileExtractor>();
                services.AddSingleton<ISerilogFileCollector, SerilogFileCollector>();

                services.AddSingleton<ILogAnalyzer, LogAnalyzer.LogAnalyzer>();
                services.AddSingleton<IGitHubReleaseService, GitHubReleaseService>();
                services.AddSingleton<ICheckForUpdatesBoxDialogViewModel, CheckForUpdatesBoxDialogViewModel>();
                services.AddSingleton<StatusBarViewModel>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        logger = host.Services.GetService<ILoggerFactory>()!.CreateLogger<App>();
    }

    /// <summary>
    /// Raises the Startup event.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
    protected override void OnStartup(
        StartupEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        logger.LogInformation("App initializing");

        // Hook on error before app really starts
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        Current.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
        base.OnStartup(e);

        if (Debugger.IsAttached)
        {
            BindingErrorTraceListener.StartTrace();
        }
    }

    /// <summary>
    /// Currents the domain unhandled exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
    private void CurrentDomainUnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        if (e is not { ExceptionObject: Exception ex })
        {
            return;
        }

        var exceptionMessage = ex.GetMessage(includeInnerMessage: true);

        logger.LogError($"CurrentDomain Unhandled Exception: {exceptionMessage}");

        MessageBox.Show(
            exceptionMessage,
            "CurrentDomain Unhandled Exception",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <summary>
    /// Applications the on dispatcher unhandled exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
    private void ApplicationOnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        var exceptionMessage = e.Exception.GetMessage(includeInnerMessage: true);
        if (exceptionMessage.Contains(
                "BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'ComboBoxItem'",
                StringComparison.Ordinal))
        {
            e.Handled = true;
            return;
        }

        logger.LogError($"Dispatcher Unhandled Exception: {exceptionMessage}");

        MessageBox.Show(
            exceptionMessage,
            "Dispatcher Unhandled Exception",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
        Shutdown(-1);
    }

    private async void ApplicationStartup(
        object sender,
        StartupEventArgs e)
    {
        logger.LogInformation("App starting");

        await host
            .StartAsync()
            .ConfigureAwait(continueOnCapturedContext: false);

        var applicationOptions = new BasicApplicationOptions();
        configuration!
            .GetRequiredSection(BasicApplicationOptions.SectionName)
            .Bind(applicationOptions);

        CultureManager.SetCultures("en-US");

        ThemeManagerHelper.SetThemeAndAccent(Current, applicationOptions.Theme);

        if (!Directory.Exists(LogViewerCommonApplicationDataDirectory.FullName))
        {
            Directory.CreateDirectory(LogViewerCommonApplicationDataDirectory.FullName);
        }

        if (!Directory.Exists(LogViewerProgramDataProfilesDirectory.FullName))
        {
            Directory.CreateDirectory(LogViewerProgramDataProfilesDirectory.FullName);
        }

        Current.MainWindow = host
            .Services
            .GetService<MainWindow>()!;

        Current.MainWindow.Show();

        logger.LogInformation("App started");
    }

    private async void ApplicationExit(
        object sender,
        ExitEventArgs e)
    {
        logger.LogInformation("App closing");

        await host
            .StopAsync()
            .ConfigureAwait(continueOnCapturedContext: false);

        host.Dispose();

        logger.LogInformation("App closed");
    }

    private static void RestoreInstallerCustomAppSettingsIfNeeded()
    {
        var currentFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AtcFileNameConstants.AppSettingsCustom));
        var backupFile = new FileInfo(Path.Combine(LogViewerCommonApplicationDataDirectory.FullName, AtcFileNameConstants.AppSettingsCustom));
        if (!currentFile.Exists ||
            !backupFile.Exists ||
            currentFile.LastWriteTime == backupFile.LastWriteTime)
        {
            return;
        }

        File.Copy(backupFile.FullName, currentFile.FullName, overwrite: true);
    }
}