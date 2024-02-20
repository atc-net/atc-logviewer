namespace Atc.LogViewer.Wpf.App.UserControls;

public class StatusBarViewModel : ViewModelBase
{
    public StatusBarViewModel()
    {
        Messenger.Default.Register<LogEntriesStatsMessage>(this, OnLogEntriesStatsMessageHandler);
    }

    public LogEntriesStatsMessage? LogEntriesStats { get; set; }

    private void OnLogEntriesStatsMessageHandler(
        LogEntriesStatsMessage obj)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            LogEntriesStats = obj;

            RaisePropertyChanged(nameof(LogEntriesStats));
        });
    }
}