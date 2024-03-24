using Atc.LogCollector.NLog;
using Atc.LogCollector.Tests.NLog;

[Collection(nameof(TestCollection))]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
[SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "OK.")]
[SuppressMessage("Design", "MA0047:Declare types in namespaces", Justification = "OK.")]
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "OK.")]
public class NLogFileCollectorIntegrationTests : NLogCollectorIntegrationTestBase
{
    [Fact]
    public void CanParseFileFormat()
    {
        // Arrange
        SendLogItemsToLogger();
        FlushAndShutdownLogger();

        var extractor = new NLogFileExtractor();
        var collector = new NLogFileCollector(extractor);

        // Atc
        var actual = collector.CanParseFileFormat(
            CurrentLogFile);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task ReadAndParseLines()
    {
        // Arrange
        SendLogItemsToLogger();
        FlushAndShutdownLogger();

        var extractor = new NLogFileExtractor();
        var collector = new NLogFileCollector(extractor);

        // Atc
        var (isSuccessFul, lastLineNumber) = await collector.ReadAndParseLines(
            CurrentLogFile,
            CancellationToken.None);

        // Assert
        Assert.True(isSuccessFul);
        Assert.Equal(LogItems.Count, lastLineNumber);
    }
}