namespace Atc.LogCollector;

public class LogFileCollectorConfiguration
{
    public ushort MaxDaysBack { get; set; } = 10;

    public bool MonitorFiles { get; set; } = true;

    public IList<string> FileNameTerms { get; set; } = [];

    public override string ToString()
        => $"{nameof(MaxDaysBack)}: {MaxDaysBack}, {nameof(MonitorFiles)}: {MonitorFiles}, {nameof(FileNameTerms)}.Count: {FileNameTerms.Count}";
}