namespace Atc.LogCollector;

public class LogFileCollectorConfig
{
    public ushort MaxDaysBack { get; set; } = 10;

    public IList<IList<string>> FileNameTerms { get; set; } = [];

    public override string ToString()
        => $"{nameof(MaxDaysBack)}: {MaxDaysBack}";
}